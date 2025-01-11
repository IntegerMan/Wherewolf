using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Phases;

public class VotingPhase : GamePhase
{
    public override GameState Run(GameState newState)
    {
        newState.AddEvent(new GamePhaseAnnouncedEvent("Everyone, vote for one other player."));

        List<VotedEvent> voteEvents = new(newState.Players.Count());
        Dictionary<Player, Player> votes = new();
        foreach (var p in newState.Players)
        {
            Player vote = p.Controller.GetPlayerVote(p, newState);
            
            voteEvents.Add(new VotedEvent(p, vote));
            votes[p] = vote;
        }

        // Add all the vote events.
        // This is done after votes are gathered so later players don't see votes of earlier players until all have voted
        foreach (var vote in voteEvents)
        {
            newState.AddEvent(vote);
        }
        
        IDictionary<Player,int> votingResults = VotingHelper.GetVotingResults(votes);
        GameResult result = newState.DetermineGameResults(votingResults);
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