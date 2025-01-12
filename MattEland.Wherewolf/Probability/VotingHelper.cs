using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;

namespace MattEland.Wherewolf.Probability;

public static class VotingHelper
{
    public static Dictionary<Player, float> GetVoteVictoryProbabilities(Player player, GameState state)
    {
        IDictionary<Player, Player>[] permutations = state.Setup.VotingPermutations;

        // Build a collection of results based on whom the player voted for - for every world this player thinks might be valid
        Dictionary<Player, List<GameResult>> results = new(state.Players.Count() - 1);
        foreach (var otherPlayer in state.Players.Where(p => p != player))
        {
            results[otherPlayer] = new();
        }
        IEnumerable<StartRoleClaimedEvent> claims = state.Claims.OfType<StartRoleClaimedEvent>();
        StartRoleClaimedEvent[] otherClaims = claims.Where(e => e.Player != player).ToArray();
        
        IEnumerable<GameState> possibleStates = GetPossibleGameStatesForPlayer(player, state);
        Dictionary<Player, int> votes = new();
        foreach (var possibleState in possibleStates)
        {
            int supportingClaims = otherClaims.Count(e => e.IsClaimValidFor(possibleState));
            
            foreach (var perm in permutations)
            {
                GetVotingResults(perm, votes);
                
                GameResult gameResult = possibleState.DetermineGameResults(votes, supportingClaims);

                results[perm[player]].Add(gameResult);
            }
        }

        // Calculate win % for each
        Dictionary<Player, float> winProbability = new();
        foreach (var kvp in results)
        {
            IEnumerable<GameResult> resultsForPlayer = kvp.Value;
            
            float totalWeight = 0;
            float cumulatedValue = 0;
            
            foreach (var result in resultsForPlayer)
            {
                int weight = 1 + result.SupportingClaims;
                totalWeight += weight;
                cumulatedValue += result.WinningPlayers.Contains(player) ? weight : 0;
            }
            
            winProbability[kvp.Key] = cumulatedValue / totalWeight;
        }

        return winProbability;
    }

    public static IEnumerable<GameState> GetPossibleGameStatesForPlayer(Player player, GameState state)
    {
        GameEvent[] observedEvents = state.Events.Where(e => e.IsObservedBy(player)).ToArray();
        GamePhase? currentPhase = state.CurrentPhase;
        
        return state.Setup.GetPermutationsAtPhase(currentPhase)
            .Where(p => p.IsPossibleGivenEvents(observedEvents));
    }

    public static IDictionary<Player, int> GetVotingResults(IDictionary<Player, Player> votes)
    {
        Dictionary<Player, int> voteTotals = new();

        return GetVotingResults(votes, voteTotals);
    }

    private static IDictionary<Player, int> GetVotingResults(IDictionary<Player, Player> votes, IDictionary<Player, int> voteTotals)
    {
        // TODO: This will probably need to be revisited to support the Hunter / Bodyguard

        // Initialize everyone at 0 votes. This ensures they're in the dictionary
        for (int i = 0; i < votes.Count; i++)
        {
            voteTotals[votes.Keys.ElementAt(i)] = 0;
        }

        // Tabulate votes
        for (int i = 0; i < votes.Count; i++)
        {
            voteTotals[votes.Values.ElementAt(i)]++;
        }
        
        return voteTotals;
    }

    public static Player GetMostLikelyWinningVote(GameState world, Player votingPlayer, Random rand)
    {
        Dictionary<Player, float> probabilities = GetVoteVictoryProbabilities(votingPlayer, world);

        float maxProb = probabilities.Max(kvp => kvp.Value);

        List<KeyValuePair<Player, float>> atMax = probabilities.Where(kvp => kvp.Value >= maxProb).ToList();

        return atMax.Count == 1 
            ? atMax.First().Key 
            : atMax[rand.Next(atMax.Count)].Key;
    }
}