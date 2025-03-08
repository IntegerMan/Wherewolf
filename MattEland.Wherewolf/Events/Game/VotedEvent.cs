using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Events.Game;

public class VotedEvent(Player votingPlayer, Player targetPlayer, PlayerProbabilities? probabilities) : GameEvent
{
    public Player VotingPlayer => votingPlayer;
    public Player TargetPlayer => targetPlayer;
    
    public PlayerProbabilities? Probabilities => probabilities;

    public override bool IsObservedBy(Player player) => true;

    public override string Description => $"{VotingPlayer.Name} voted for {TargetPlayer.Name}";
    public override bool IsPossibleInGameState(GameState state) => VotingPlayer != TargetPlayer;
}