using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Phases;

public class VotingPhase : GamePhase
{
    private readonly Lock _lock = new();
    
    public override void Run(GameState newState, Action<GameState> callback)
    {
        newState.AddEvent(new GamePhaseAnnouncedEvent("Everyone, vote for one other player.", null));

        List<VotedEvent> voteEvents = new(newState.Players.Count());
        Dictionary<Player, Player> votes = new();
        foreach (var p in newState.Players)
        {
            p.Controller.GetPlayerVote(p, newState, vote => OnVoteReceived(newState, callback, voteEvents, p, vote, votes));
        }
    }

    private void OnVoteReceived(GameState newState, Action<GameState> callback, List<VotedEvent> voteEvents, Player voter, Player target,
        Dictionary<Player, Player> votes)
    {
        int voteCount;
        lock (_lock)
        {
            voteEvents.Add(new VotedEvent(voter, target));
            votes[voter] = target;
            voteCount = votes.Count;
        }
        
        if (voteCount == newState.Players.Count())
        {
            OnAllVotesReceived(newState, callback, voteEvents, votes);
        }
    }

    private static void OnAllVotesReceived(GameState newState, Action<GameState> callback, List<VotedEvent> voteEvents, Dictionary<Player, Player> votes)
    {
        // Add all the vote events.
        // This is done after votes are gathered so later players don't see votes of earlier players until all have voted
        foreach (var vote in voteEvents)
        {
            newState.AddEvent(vote);
        }
        
        IDictionary<Player,int> votingResults = VotingHelper.GetVotingResults(votes);
        GameResult result = newState.DetermineGameResults(votingResults);
        newState.GameResult = result;

        AddVoteAftermathEvents(newState, result, broadcast: true);

        callback(newState);
    }

    private static void AddVoteAftermathEvents(GameState newState, GameResult result, bool broadcast)
    {
        // Ensure we get events for voted out players
        foreach (var deadPlayer in result.DeadPlayers)
        {
            GameSlot playerSlot = newState[deadPlayer.Name];
            newState.AddEvent(new VotedOutEvent(deadPlayer, playerSlot.Role), broadcast);
        }

        // Get a final event to tell us who won the game
        newState.AddEvent(new GameOverEvent(result), broadcast);
    }

    public override double Order => double.MaxValue;
    public override string Name => "Voting";
    public override IEnumerable<GameState> BuildPossibleStates(GameState priorState)
    {
        IDictionary<Player, Player>[] allPermutations = priorState.Setup.VotingPermutations;
        
        foreach (var perm in allPermutations)
        {
            GameState newState = new(priorState, priorState.Support / allPermutations.Length);
            Dictionary<Player, int> votes = new();
            foreach (var kvp in perm)
            {
                votes[kvp.Key] = 0;
            }

            newState.AddEvent(new GamePhaseAnnouncedEvent("Everyone, vote for one other player.", null), broadcastToController: false);

            foreach (var kvp in perm)
            {
                Player voter = kvp.Key;
                Player vote = kvp.Value;
                votes[vote]++;

                newState.AddEvent(new VotedEvent(voter, vote), broadcastToController: false);
            }

            GameResult result = newState.DetermineGameResults(votes);
            newState.GameResult = result;
            
            AddVoteAftermathEvents(newState, result, broadcast: false);
            
            yield return newState;
        }
    }
}