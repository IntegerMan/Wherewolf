namespace MattEland.Wherewolf.Probability;

public record SlotProbability(double Probability, IEnumerable<Player> SupportingClaims)
{
}