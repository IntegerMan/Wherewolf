namespace MattEland.Wherewolf.Events.Game;

public class VotedEvent(Player votingPlayer, Player targetPlayer) : GameEvent
{
    public Player VotingPlayer { get; } = votingPlayer;
    public Player TargetPlayer { get; } = targetPlayer;

    public override bool IsObservedBy(Player player) => true;

    public override string Description => $"{VotingPlayer.Name} voted for {TargetPlayer.Name}";
    public override bool IsPossibleInGameState(GameState state) => VotingPlayer != TargetPlayer;
}