namespace MattEland.Wherewolf.Probability;

public record VoteVictoryStatistics
{
    public double Support { get; set; }
    public double WinningSupport { get; set; }

    public override string ToString() => $"Wins: {WinningSupport} / Support: {Support}";

    public double WinPercent => WinningSupport / Support;
}