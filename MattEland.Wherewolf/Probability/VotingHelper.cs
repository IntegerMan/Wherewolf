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
    
    private static float CalculatePlayerWinPercentWithBestVotingOption(Player player, GameState state)
    {
        IDictionary<Player, IDictionary<Player, float>> playerVoteProbabilities = BuildPlayerWeightedVoteProbabilities(state);

        int supportingClaims = state.Claims.OfType<StartRoleClaimedEvent>()
            .Count(e => e.Player != player && e.IsClaimValidFor(state));
        
        // Loop through each combination of votes and calculate the win probability for the player
        float winWeight = 0f;
        float totalWeight = 0f;
        Dictionary<Player, int> votes = new();
        foreach (var perm in state.Setup.VotingPermutations)
        {
            GetVotingResults(perm, votes);
            GameResult gameResult = state.DetermineGameResults(votes, supportingClaims);
            
            float weight = 0f; // TODO: The weight here might want to also include supportingClaims
            foreach (var kvp in perm)
            {
                weight += playerVoteProbabilities[kvp.Key][kvp.Value];
            }

            totalWeight += weight;
            if (gameResult.WinningPlayers.Contains(player))
            {
                winWeight += weight;
            }
        }
        
        return winWeight / totalWeight;
    }

    private static IDictionary<Player, IDictionary<Player, float>> BuildPlayerWeightedVoteProbabilities(GameState state)
    {
        // Build a set of probabilities for each player voting for each other player
        Dictionary<Player, IDictionary<Player, float>> playerVoteProbabilities = new();
        foreach (var votingPlayer in state.Players)
        {
            playerVoteProbabilities[votingPlayer] = GetVoteVictoryProbabilities(votingPlayer, state);
        }

        return playerVoteProbabilities;
    }


    public static float GetStateVictoryPercent(Player player, GameState state)
    {
        // Identify all possible current game states, given what the player knows
        GameEvent[] knownEvents = state.Events.Where(e => e.IsObservedBy(player))
            .ToArray();
        var permutations = state.Setup.GetPermutationsAtPhase(state.CurrentPhase)
            .Where(p => p.IsPossibleGivenEvents(knownEvents));
        
        // Given this permutation mix, figure out how often this path results in victory for the player
        int totalWeight = 0;
        float winPercent = 0;
        foreach (var perm in permutations)
        {
            int permWeight = 1 + perm.ObservedSupport(player);
            winPercent += CalculatePlayerWinPercentWithBestVotingOption(player, perm) * permWeight;
            totalWeight += permWeight;
        }
        
        // Sanity check for divide by zero scenarios (no permutations found for player)
        if (totalWeight <= 0)
            throw new InvalidOperationException("No possible game states found for player " + player.Name);
        
        // Return the average win probability for the player, taking probability of scenarios given claims into account
        return winPercent / totalWeight;
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
    
    public static IDictionary<string, VoteVictoryStatistics> BuildOtherPlayerVoteVictoryStatistics(Player player, GameState gameState)
    {
        IDictionary<string, VoteVictoryStatistics> stats = new Dictionary<string, VoteVictoryStatistics>(); 
        IDictionary<Player, Player>[] votePermutations = gameState.Setup.VotingPermutations;
        GameEvent[] observedEvents = gameState.Events.Where(e => e.IsObservedBy(player)).ToArray();

        IEnumerable<GameState> statePermutations = gameState.Setup.GetPermutationsAtPhase(gameState.CurrentPhase);
        IEnumerable<GameState> possiblePermutations = statePermutations.Where(p => p.IsPossibleGivenEvents(observedEvents));
        
        foreach (var state in possiblePermutations)
        {
            foreach (var perm in votePermutations)
            {
                IDictionary<Player, int> votes = GetVotingResults(perm);
                GameResult result = state.DetermineGameResults(votes);
                
                bool isWinning = result.WinningPlayers.Contains(player);
                
                foreach (var kvp in perm)
                {
                    Player votingPlayer = kvp.Key;
                    Player votedPlayer = kvp.Value;
                    
                    if (votingPlayer == player)
                    {
                        continue;
                    }

                    string key = $"{votingPlayer}_{votedPlayer}";
                    if (!stats.TryGetValue(key, out var stat))
                    {
                        stats[key] = new VoteVictoryStatistics
                        {
                            Support = 1,
                            Wins = isWinning ? 1 : 0
                        };
                    }
                    else
                    {
                        stat.Support++;
                        if (isWinning)
                        {
                            stat.Wins++;
                        }
                    }
                }
            }
        }

        return stats;
    }
}