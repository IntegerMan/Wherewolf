namespace MattEland.Wherewolf.Probability;

public record VoteVictoryStatistics
{
    public int Support { get; set; }
    public int Wins { get; set; }

    public override string ToString()
    {
        return $"Wins: {Wins} / Support: {Support}";
    }

    public double WinPercent => (double) Wins / Support;
}