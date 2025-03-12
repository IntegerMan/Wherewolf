using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Events.Game;

// TODO: This would be better to use the EventPool, but it can't at the moment since PlayerProbabilities is a specific object

public class VotedEvent(Player votingPlayer, Player targetPlayer, PlayerProbabilities? probabilities) : GameEvent
{
    public Player VotingPlayer { get; } = votingPlayer;

    public Player TargetPlayer { get; } = targetPlayer;

    public PlayerProbabilities? Probabilities { get; } = probabilities;

    public override bool IsObservedBy(Player player) => true;

    public override string Description => $"{VotingPlayer.Name} voted for {TargetPlayer.Name}";
    public override bool IsPossibleInGameState(GameState state) => VotingPlayer != TargetPlayer;
}