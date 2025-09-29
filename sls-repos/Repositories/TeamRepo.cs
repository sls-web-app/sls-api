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
}

