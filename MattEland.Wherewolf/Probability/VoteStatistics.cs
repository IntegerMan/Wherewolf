namespace MattEland.Wherewolf.Probability;

public record VoteStatistics
{
    public double Support { get; set; }
    public int VotesReceived { get; set; }

    public override string ToString()
    {
        return $"Votes Received: {VotesReceived}, Games: {Games}, Support: {Support}";
    }

    public double VotesPerGame => VotesReceived / Support;
    public double InPlayPercent { get; set; }
    public double OutOfPlayPercent { get; set; }
    public int Games { get; set; }
}