using sls_borders.DTO.UserDto;

namespace sls_utils.BuchholzUtils;

public static class BuchholzCalculator
{
    public static int CalculateFullBuchholzScore(List<UserInPlay> players, HashSet<Guid> opponentsIds)
    {
        int opponentsScores = 0;
        var opponents = players.Where(p => opponentsIds.Contains(p.Id)).ToList();

        foreach (var opponent in opponents) opponentsScores += opponent.Wins * 2 + opponent.Draws;

        return opponentsScores;
    }

    public static int CalculateMedianBuchholzScore(List<UserInPlay> players, HashSet<Guid> opponentsIds)
    {
        var opponentsScores = new List<int>();
        var opponents = players.Where(p => opponentsIds.Contains(p.Id)).ToList();

        foreach (var opponent in opponents)
        {
            int score = opponent.Wins * 2 + opponent.Draws;
            opponentsScores.Add(score);
        }

        if (opponentsScores.Count <= 2)
            return opponentsScores.Sum();

        int lowestScore = opponentsScores.Min();
        int highestScore = opponentsScores.Max();
        opponentsScores = opponentsScores.Where(score => score != lowestScore && score != highestScore).ToList();

        return opponentsScores.Sum();
    }
}