namespace MattEland.Wherewolf.Probability;

public record VoteStatistics
{
    public double Support { get; set; }
    public int VotesReceived { get; set; }

    public override string ToString()
    {
        return $"Votes Received: {VotesReceived} / Games: {Support}";
    }

    public double VotesPerGame => VotesReceived / Support;
}