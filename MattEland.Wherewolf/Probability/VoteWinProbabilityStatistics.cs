namespace MattEland.Wherewolf.Probability;

public record VoteWinProbabilityStatistics
{
    public int Support { get; set; }
    public double TotalWinProbability { get; set; }

    public override string ToString()
    {
        return $"Win Probability: {WinPercent:P2} given Support: {Support}";
    }

    public double WinPercent => TotalWinProbability/ Support;
}