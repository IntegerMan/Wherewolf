namespace MattEland.Wherewolf.Probability;

public record VoteStatistics
{
    public int Support { get; set; }
    public int Wins { get; set; }

    public override string ToString()
    {
        return $"Wins: {Wins} / Support: {Support}, Win %: {WinPercent:P2}";
    }

    public double WinPercent => (double) Wins / Support;
}