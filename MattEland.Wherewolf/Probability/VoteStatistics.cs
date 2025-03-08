namespace MattEland.Wherewolf.Probability;

public record VoteStatistics
{
    public double Support { get; set; }
    public int VotesReceived { get; set; }

    public override string ToString()
    {
        return $"Votes Received: {VotesReceived}, Games: {Games}, Support: {Support}";
    }

    public int VoteFactor => (int)Math.Round(VotesReceived / Support * 10.0);
    public double InPlayPercent { get; set; }
    public double OutOfPlayPercent { get; set; }
    public int Games { get; set; }
    public int OtherClaims { get; init; }
}