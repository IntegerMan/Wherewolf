namespace MattEland.Wherewolf.Probability;

public record VoteWinProbabilityStatistics
{
    public int Support { get; set; }
    public float TotalWinProbability { get; set; }

    public override string ToString()
    {
        return $"Win Probability: {WinPercent:P2} given Support: {Support}";
    }

    public double WinPercent => TotalWinProbability/ Support;
}