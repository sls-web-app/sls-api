using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using sls_borders.DTO.GameDto;
using sls_borders.Enums;
using sls_borders.Repositories;

namespace sls_api.Hubs;

// [Authorize]
public class TournamentsHub(ITournamentRepo tournamentRepo, IGameRepo gameRepo) : Hub
{
    public async Task JoinTournamentGroup(Guid tournamentId)
    {
        var tournament = await tournamentRepo.GetByIdAsync(tournamentId);
        if (tournament == null)
        {
            throw new InvalidOperationException($"Tournament with ID {tournamentId} not found.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, tournamentId.ToString());
    }

    public async Task UpdateGameScore(Guid gameId, GameScore score)
    {
        var updatedGame = await gameRepo.UpdateScoreAsync(gameId, score);
        if (updatedGame == null)
        {
            throw new InvalidOperationException($"Game with ID {gameId} not found.");
        }

        await Clients.Group(updatedGame.TournamentId.ToString())
            .SendAsync("GameScoreUpdated", new UpdatedScoreDto
            {
                Id = updatedGame.Id,
                Score = updatedGame.Score
            });
    }

    public async Task AdvanceTournamentRound(Guid tournamentId)
    {
        var games = await tournamentRepo.AdvandeToNextRoundAsync(tournamentId);
        if (games == null)
        {
            throw new InvalidOperationException($"Tournament with ID {tournamentId} not found or cannot advance.");
        }

        await Clients.Group(tournamentId.ToString()).SendAsync("TournamentRoundAdvanced", games);
    }
}