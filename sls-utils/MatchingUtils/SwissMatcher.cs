using System;
using System.Collections.Generic;
using System.Linq;
using sls_borders.Enums;
using sls_borders.Models;

namespace sls_utils.MatchingUtils
{
    public class MatchPlayer
    {
        public Guid Id { get; set; }
        public double Score { get; set; }
        public HashSet<Guid> Opponents { get; } = new();
        public List<char> Colors { get; } = new();
        public bool HadBye { get; set; } = false;

        public MatchPlayer(Guid id, HashSet<Guid>? opponents = null, List<char>? colors = null, double score = 0.0, bool hadBye = false)
        {
            Id = id;
            Opponents = opponents ?? new HashSet<Guid>();
            Colors = colors ?? new List<char>();
            Score = score;
            HadBye = hadBye;
        }

        public int ColorBalance() => Colors.Count(c => c == 'W') - Colors.Count(c => c == 'B');
        public int WhiteCount() => Colors.Count(c => c == 'W');
        public int BlackCount() => Colors.Count(c => c == 'B');

        public bool HasThreeSameColor(char color)
        {
            if (Colors.Count < 2) return false;
            return Colors.TakeLast(2).All(c => c == color);
        }

    }

    public class Match
    {
        public MatchPlayer White { get; }
        public MatchPlayer? Black { get; }
        public bool IsBye => Black == null;

        public Match(MatchPlayer white, MatchPlayer? black)
        {
            White = white;
            Black = black;
        }
    }

    public class SwissTournament
    {
        private readonly List<MatchPlayer> _players;
        public int _currentRound = 0;

        public SwissTournament(List<MatchPlayer> players, int rounds)
        {
            if (players.Count > 50) throw new ArgumentException("Max 50 players allowed");
            _players = players;
            _currentRound = rounds;
        }

        public List<Match> NextRound()
        {

            _currentRound++;
            Console.WriteLine($"\n--- Round {_currentRound} ---");

            var sorted = _players
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.Id)
                .ToList();

            var matches = new List<Match>();

            // BYE
            if (sorted.Count % 2 == 1)
            {
                var byeCandidate = sorted.LastOrDefault(p => !p.HadBye);
                if (byeCandidate == null) byeCandidate = sorted.Last();
                byeCandidate.HadBye = true;
                byeCandidate.Score += 1.0;
                matches.Add(new Match(byeCandidate, null));
                sorted.Remove(byeCandidate);
            }

            // Grupowanie po punktach
            var groups = sorted.GroupBy(p => p.Score)
                               .OrderByDescending(g => g.Key)
                               .Select(g => g.ToList())
                               .ToList();

            var allPairs = new List<Match>();
            PairGroups(groups, 0, allPairs);
            matches.AddRange(allPairs);

            return matches;
        }

        private bool PairGroups(List<List<MatchPlayer>> groups, int idx, List<Match> output)
        {
            if (idx >= groups.Count) return true;

            var group = groups[idx];
            if (group.Count == 0) return PairGroups(groups, idx + 1, output);

            if (group.Count % 2 == 1)
            {
                // trzeba floatować
                var floater = group.Last(); // prostsza reguła: ostatni
                groups[idx].Remove(floater);
                if (idx + 1 < groups.Count)
                    groups[idx + 1].Add(floater);
                else
                    groups.Add(new List<MatchPlayer> { floater });
            }

            var pairs = new List<Match>();
            if (!PairGroupBacktrack(group, pairs))
                return false;

            output.AddRange(pairs);
            return PairGroups(groups, idx + 1, output);
        }

        private bool PairGroupBacktrack(List<MatchPlayer> players, List<Match> matches)
        {
            if (players.Count == 0) return true;

            var p1 = players[0];
            for (int i = 1; i < players.Count; i++)
            {
                var p2 = players[i];
                if (p1.Opponents.Contains(p2.Id)) continue;

                var (white, black) = AssignColors(p1, p2);
                if (white.HasThreeSameColor('W') || black.HasThreeSameColor('B'))
                    continue; // łamiemy zasadę 3x kolor

                var match = new Match(white, black);

                // aktualizujemy stan
                p1.Opponents.Add(p2.Id);
                p2.Opponents.Add(p1.Id);
                white.Colors.Add('W');
                black.Colors.Add('B');

                matches.Add(match);

                var rest = players.Where(p => p != p1 && p != p2).ToList();
                if (PairGroupBacktrack(rest, matches)) return true;

                // rollback
                matches.Remove(match);
                white.Colors.RemoveAt(white.Colors.Count - 1);
                black.Colors.RemoveAt(black.Colors.Count - 1);
                p1.Opponents.Remove(p2.Id);
                p2.Opponents.Remove(p1.Id);
            }
            return false;
        }

        private static (MatchPlayer white, MatchPlayer black) AssignColors(MatchPlayer a, MatchPlayer b)
        {
            var balA = a.ColorBalance();
            var balB = b.ColorBalance();

            if (balA > balB) return (b, a);
            if (balB > balA) return (a, b);

            if (a.WhiteCount() < b.WhiteCount()) return (a, b);
            if (b.WhiteCount() < a.WhiteCount()) return (b, a);

            return (Random.Shared.Next(2) == 0) ? (a, b) : (b, a);
        }
    }

    public class SwissMatcher
    {
        public static List<Game> GenerateGamesForSwissTournament(Tournament tournament)
        {
        var players = tournament.Edition.Teams
                    .SelectMany(t => t.Users)
                    .Where(u => u.IsInPlay)
                    .ToList();
        var matchPlayers = new List<MatchPlayer>();

        foreach (var player in players) // Wiem że to nieoptymalne, ale trudno
        {
            var opponents = tournament.Games
                .Where(g => (g.WhitePlayerId == player.Id || g.BlackPlayerId == player.Id) && g.Score != null)
                .Select(g => g.WhitePlayerId == player.Id ? g.BlackPlayerId : g.WhitePlayerId)
                .ToHashSet();

            var colors = tournament.Games
                .Where(g => (g.WhitePlayerId == player.Id || g.BlackPlayerId == player.Id) && g.Score != null)
                .Select(g => g.WhitePlayerId == player.Id ? 'W' : 'B')
                .ToList();

            var playerScore = tournament.Games
                .Where(g => (g.WhitePlayerId == player.Id && g.Score == GameScore.WhiteWin) || (g.BlackPlayerId == player.Id && g.Score == GameScore.BlackWin))
                .Count() * 1.0 +
                tournament.Games.Where(g => (g.WhitePlayerId == player.Id || g.BlackPlayerId == player.Id) && g.Score == GameScore.Draw)
                .Count() * 0.5;
            
            var hadBye = tournament.Games
                .Where(g => g.WhitePlayerId == player.Id && g.BlackPlayerId == Guid.Empty)
                .Any();

            matchPlayers.Add(new MatchPlayer(player.Id, opponents, colors, playerScore, hadBye));
        }

        var swissTournament = new SwissTournament(matchPlayers, tournament.Round ?? 1); 
        var matches = swissTournament.NextRound();

        var games = new List<Game>();
        foreach (var match in matches)
        {
            if (match.IsBye)
            {
                games.Add(new Game
                {
                    TournamentId = tournament.Id,
                    WhitePlayerId = match.White.Id,
                    BlackPlayerId = Guid.Empty,
                    WhiteTeamId = tournament.Edition.Teams.FirstOrDefault(t => t.Users.Any(u => u.Id == match.White.Id))?.Id ?? Guid.Empty,
                    BlackTeamId = Guid.Empty,
                    Round = tournament.Round ?? 1
                });
                continue;
            }

            games.Add(new Game
            {
                TournamentId = tournament.Id,
                WhitePlayerId = match.White.Id,
                BlackPlayerId = match.Black!.Id,
                WhiteTeamId = tournament.Edition.Teams.FirstOrDefault(t => t.Users.Any(u => u.Id == match.White.Id))?.Id ?? Guid.Empty,
                BlackTeamId = tournament.Edition.Teams.FirstOrDefault(t => t.Users.Any(u => u.Id == match.Black!.Id))?.Id ?? Guid.Empty,
                Round = tournament.Round ?? 1
            });
        }

        return games;
    }
    }
}
