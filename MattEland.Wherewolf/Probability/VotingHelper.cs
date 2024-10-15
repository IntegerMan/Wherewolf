using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Probability;

public static class VotingHelper
{
    public static Dictionary<Player, float> GetVoteVictoryProbabilities(Player player, GameState state)
    {
        List<Dictionary<Player, Player>> permutations = state.Setup.GetVotingPermutations().ToList();

        // Build a collection of results based on who the player voted for - for every world this player thinks might be valid
        Dictionary<Player, List<GameResult>> results = new();
        foreach (var possibleState in GetPossibleGameStatesForPlayer(player, state))
        {
            AddGameStateVoteResultPossibilities(possibleState, player, permutations, results);
        }

        // Calculate win % for each
        Dictionary<Player, float> winProbability = new();
        foreach (var kvp in results)
        {
            winProbability[kvp.Key] = kvp.Value.Average(r => r.WinningPlayers.Contains(player) ? 1f : 0f);
        }

        return winProbability;
    }

    public static List<GameState> GetPossibleGameStatesForPlayer(Player player, GameState state)
    {
        List<GameEvent> observedEvents = state.Events.Where(e => e.IsObservedBy(player)).ToList();
        GamePhase? currentPhase = state.CurrentPhase;
        List<GameState> phasePermutations = state.Setup.GetPermutationsAtPhase(currentPhase).ToList();
        if (!phasePermutations.Any())
            throw new InvalidOperationException("No phase permutations found for phase " + (currentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        List<GameState> validPermutations = phasePermutations.Where(p => p.IsPossibleGivenEvents(observedEvents)).ToList();
        if (!validPermutations.Any())
            throw new InvalidOperationException("No valid permutations found for phase " + (currentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        return validPermutations;
    }    
    
    private static void AddGameStateVoteResultPossibilities(GameState state, Player player, IEnumerable<Dictionary<Player, Player>> permutations, Dictionary<Player, List<GameResult>> results)
    {
        foreach (var perm in permutations)
        {
            Dictionary<Player, int> votes = GetVotingResults(perm);
            
            GameResult gameResult = state.DetermineGameResults(votes);

            Player action = perm[player];
            if (!results.ContainsKey(action))
            {
                results[action] = [gameResult];
            }
            else
            {
                results[action].Add(gameResult);
            }
        }
    }    
    
    public static Dictionary<Player, int> GetVotingResults(Dictionary<Player, Player> votes)
    {
        // TODO: This will probably need to be revisited to support the Hunter / Bodyguard

        Dictionary<Player, int> voteTotals = new();
        
        // Initialize everyone at 0 votes. This ensures they're in the dictionary
        foreach (var player in votes.Keys)
        {
            voteTotals[player] = 0;
        }

        // Tabulate votes
        foreach (var target in votes.Values)
        {
            voteTotals[target]++;
        }
        
        return voteTotals;
    }

    public static Player GetMostLikelyWinningVote(GameState world, Player votingPlayer)
    {
        Dictionary<Player, float> probabilities = GetVoteVictoryProbabilities(votingPlayer, world);
        
        return probabilities.MaxBy(kvp => kvp.Value).Key;
    }
}