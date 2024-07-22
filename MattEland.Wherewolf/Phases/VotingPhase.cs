using MattEland.Wherewolf.Events;

namespace MattEland.Wherewolf.Phases;

public class VotingPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        Dictionary<Player, Player> votes = new();
        foreach (var p in newState.Players)
        {
            Player vote = p.Controller.GetPlayerVote(p, newState);
            
            newState.AddEvent(new VotedEvent(p, vote));
            votes[p] = vote;
        }

        GameResult result = newState.DetermineGameResults(votes);
        newState.GameResult = result;
        
        return newState;
    }

    public override double Order => double.MaxValue;
    public override string Name => "Voting";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        yield break; // TODO: Should this give all permutations?
    }
}