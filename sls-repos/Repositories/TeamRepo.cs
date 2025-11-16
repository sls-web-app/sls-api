using AutoMapper;
using MailKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using sls_borders.Data;
using sls_borders.DTO.TeamDto;
using sls_borders.Enums;
using sls_borders.Models;
using sls_borders.Repositories;
using sls_utils.ImageUtils;

namespace sls_repos.Repositories;

public class TeamRepo(ApplicationDbContext context, IMapper mapper, IImageService imageService) : ITeamRepo
{
    public async Task<List<Team>> GetAllAsync()
    {
        return await context.Teams.ToListAsync();
    }

    public async Task<Team?> GetByIdAsync(Guid id)
    {
        return await context.Teams.FindAsync(id);
    }

    public async Task<List<Team>?> GetAllTeamsInCurrentEditionAsync()
    {
        var currentEdition = await context.Editions.Where(e => e.IsActive).FirstOrDefaultAsync();

        if (currentEdition == null)
            return null;

        // return teams that are in the current edition
        return await context.Teams
            .Where(t => t.Editions.Any(e => e.Id == currentEdition.Id))
            .ToListAsync();
    }

    public async Task<Team> CreateAsync(Team team)
    {
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        return team;
    }

    public async Task<Team?> UpdateAsync(Guid id, UpdateTeamDto updateTeamDto, IFormFile? avatar)
    {
        var existingTeam = await context.Teams.FindAsync(id);

        if (existingTeam == null)
            return null;

        // Handle avatar upload if provided
        if (avatar != null)
        {
            // Delete the old avatar if it exists
            if (!string.IsNullOrEmpty(existingTeam.Img))
            {
                var (fileName, category) = imageService.GetNameFromUrl(existingTeam.Img);
                await imageService.DeleteImageAsync(fileName, category);
            }

            using var stream = avatar.OpenReadStream();
            var uploadResult = await imageService.UploadImageAsync(
                stream,
                avatar.FileName,
                avatar.ContentType,
                ImageCategory.Avatar);

            if (!uploadResult.Success)
            {
                return null;
            }

            // Set the image URL in the DTO
            existingTeam.Img = uploadResult.ImageUrl;
        }

        mapper.Map(updateTeamDto, existingTeam);

        await context.SaveChangesAsync();
        return existingTeam;
    }

    public async Task<Team> GetTeamTournamentsInfo(Guid teamId)
    {
        var team = await context.Teams.FindAsync(teamId);

        if (team == null)
            throw new KeyNotFoundException($"Team with ID {teamId} not found.");

        return team;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var team = await context.Teams.FindAsync(id);
        if (team == null) return false;

        context.Teams.Remove(team);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> JoinEditonAsync(Guid teamId, Guid editionId)
    {
        var team = await context.Teams.FindAsync(teamId);
        if (team == null) return false;

        var edition = await context.Editions.FindAsync(editionId);
        if (edition == null) return false;

        team.Editions.Add(edition);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<GetTeamsScoreDto> GetTeamsScoreInEditionAsync(Guid editionId)
    {
        // fetch teams that participate in the edition
        var teams = await context.Editions.Where(e => e.Id == editionId)
            .SelectMany(e => e.Teams)
            .ToListAsync();

        // initialize score DTOs and dictionary for fast lookup
        var teamsWithPoints = mapper.Map<List<GetTeamWithPointsDto>>(teams);
        var teamsDict = teamsWithPoints.ToDictionary(t => t.Id);

        // load relevant games (only for tournaments in this edition) with score
        var games = await context.Tournaments
            .Where(t => t.EditionId == editionId)
            .SelectMany(t => t.Games)
            .Where(g => g.Score != null)
            .ToListAsync();

        // SMALL POINTS: count amount of games won by the team (draws add 1, wins add 2) - keep existing weights
        foreach (var g in games)
        {
            if (g.Score == GameScore.WhiteWin)
            {
                if (teamsDict.TryGetValue(g.WhiteTeamId, out var dto)) dto.SmallPoints += 2;
            }
            else if (g.Score == GameScore.BlackWin)
            {
                if (teamsDict.TryGetValue(g.BlackTeamId, out var dto)) dto.SmallPoints += 2;
            }
            else if (g.Score == GameScore.Draw)
            {
                if (teamsDict.TryGetValue(g.WhiteTeamId, out var dtoW)) dtoW.SmallPoints += 1;
                if (teamsDict.TryGetValue(g.BlackTeamId, out var dtoB)) dtoB.SmallPoints += 1;
            }
        }

        // BIG POINTS: for each tournament, compute standings based on amount of games won in that tournament
        var tournamentsGroups = games.GroupBy(g => g.TournamentId);

        foreach (var tg in tournamentsGroups)
        {
            // teams participating in this tournament: include all edition teams so teams with no games are present
            var participatingTeamIds = tg.SelectMany(g => new[] { g.WhiteTeamId, g.BlackTeamId }).Distinct().ToList();
            foreach (var id in teams.Select(t => t.Id))
            {
                if (!participatingTeamIds.Contains(id))
                    participatingTeamIds.Add(id);
            }

            // wins per team in this tournament
            var winsPerTeam = participatingTeamIds.ToDictionary(id => id, id => 0);

            foreach (var g in tg)
            {
                if (g.Score == GameScore.WhiteWin)
                {
                    winsPerTeam[g.WhiteTeamId] = winsPerTeam.GetValueOrDefault(g.WhiteTeamId) + 2;
                }
                else if (g.Score == GameScore.BlackWin)
                {
                    winsPerTeam[g.BlackTeamId] = winsPerTeam.GetValueOrDefault(g.BlackTeamId) + 2;
                }
                else if (g.Score == GameScore.Draw)
                {
                    winsPerTeam[g.WhiteTeamId] = winsPerTeam.GetValueOrDefault(g.WhiteTeamId) + 1;
                    winsPerTeam[g.BlackTeamId] = winsPerTeam.GetValueOrDefault(g.BlackTeamId) + 1;
                }
            }

            // order teams by wins desc
            var ordered = winsPerTeam.OrderByDescending(kv => kv.Value).ToList();
            int teamsCount = ordered.Count;

            int index = 0;
            int? prevWins = null;
            int prevPoints = 0;

            foreach (var kv in ordered)
            {
                int wins = kv.Value;
                int pointsForPosition;

                if (prevWins.HasValue && wins == prevWins.Value)
                {
                    // same points as previous (tie)
                    pointsForPosition = prevPoints;
                }
                else
                {
                    // higher wins -> higher points: first gets teamsCount, second gets teamsCount-1, etc.
                    pointsForPosition = teamsCount - index;
                    prevPoints = pointsForPosition;
                    prevWins = wins;
                }

                if (teamsDict.TryGetValue(kv.Key, out var dto)) dto.BigPoints += pointsForPosition;
                index++;
            }
        }

        return new GetTeamsScoreDto
        {
            Teams = teamsWithPoints
        };
    }
}

