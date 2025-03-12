namespace MattEland.Wherewolf.Events.Game;

public class MakeSocialClaimsNowEvent : GamePhaseAnnouncedEvent
{
    internal MakeSocialClaimsNowEvent() : base("Everyone, wake up and claim your starting role")
    {
    }
}