using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public static class VotingHelper
{
    public static Dictionary<Player, float> GetVoteVictoryProbabilities(Player player, GameState state)
    {
        List<Dictionary<Player, Player>> permutations = state.Setup.GetVotingPermutations().ToList();

        // Build a collection of results based on whom the player voted for - for every world this player thinks might be valid
        Dictionary<Player, List<GameResult>> results = new(state.Players.Count() - 1);
        foreach (var otherPlayer in state.Players.Where(p => p != player))
        {
            results[otherPlayer] = new();
        }
        IEnumerable<StartRoleClaimedEvent> claims = state.Claims;
        
        IEnumerable<GameState> possibleStates = GetPossibleGameStatesForPlayer(player, state);
        Dictionary<Player, int> votes = new();
        foreach (var possibleState in possibleStates)
        {
            int supportingClaims = claims.Count(e => e.Player != player && e.IsClaimValidFor(possibleState));
            
            foreach (var perm in (IEnumerable<Dictionary<Player, Player>>)permutations)
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
        // Build a set of probabilities for each player voting for each other player
        Dictionary<Player, Dictionary<Player, float>> playerVoteProbabilities = new();
        foreach (var votingPlayer in state.Players)
        {
            playerVoteProbabilities[votingPlayer] = GetVoteVictoryProbabilities(votingPlayer, state);
        }
        
        int supportingClaims = state.Claims.Count(e => e.Player != player && e.IsClaimValidFor(state));
        
        // Loop through each combination of votes and calculate the win probability for the player
        float winWeight = 0f;
        float totalWeight = 0f;
        Dictionary<Player, int> votes = new();
        foreach (var perm in state.Setup.GetVotingPermutations())
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
    
    public static float GetRoleClaimWinProbabilityPerception(Player player, GameState state, GameRole role)
    {
        // Identify all possible current game states, given what the player knows
        List<GameEvent> knownEvents = state.Events.Where(e => e.IsObservedBy(player))
            .ToList();
        List<GameState> permutations = state.Setup.GetPermutationsAtPhase(state.CurrentPhase)
            .Where(p => p.IsPossibleGivenEvents(knownEvents))
            .ToList();
        
        // Given this permutation mix, figure out how often this role claim results in victory
        int totalWeight = 0;
        float winPercent = 0;
        foreach (var perm in permutations)
        {
            GameState possibleWorld = new GameState(perm, perm.Support);
            possibleWorld.AddEvent(new StartRoleClaimedEvent(player, role), broadcastToController: false);
            
            int permWeight = 1 + possibleWorld.ObservedSupport(player);
            winPercent += CalculatePlayerWinPercentWithBestVotingOption(player, possibleWorld) * permWeight;
            totalWeight += permWeight;
        }
        
        // Sanity check for divide by zero scenarios (no permutations found for player)
        if (totalWeight <= 0)
            throw new InvalidOperationException("No possible game states found for player " + player.Name);
        
        // Return the average win probability for the player, taking probability of scenarios given claims into account
        return winPercent / totalWeight;
    }    
    
    
    public static float GetStateVictoryPercent(Player player, GameState state)
    {
        // Identify all possible current game states, given what the player knows
        List<GameEvent> knownEvents = state.Events.Where(e => e.IsObservedBy(player))
            .ToList();
        List<GameState> permutations = state.Setup.GetPermutationsAtPhase(state.CurrentPhase)
            .Where(p => p.IsPossibleGivenEvents(knownEvents))
            .ToList();
        
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
        IEnumerable<GameEvent> observedEvents = state.Events.Where(e => e.IsObservedBy(player));
        GamePhase? currentPhase = state.CurrentPhase;
        IEnumerable<GameState> phasePermutations = state.Setup.GetPermutationsAtPhase(currentPhase);
        if (!phasePermutations.Any())
            throw new InvalidOperationException("No phase permutations found for phase " + (currentPhase?.Name ?? "Voting") + " for player " + player.Name);

        IEnumerable<GameState> validPermutations = phasePermutations.Where(p => p.IsPossibleGivenEvents(observedEvents));
        if (!validPermutations.Any())
            throw new InvalidOperationException("No valid permutations found for phase " + (currentPhase?.Name ?? "Voting") + " for player " + player.Name);
        
        return validPermutations;
    }

    public static Dictionary<Player, int> GetVotingResults(Dictionary<Player, Player> votes)
    {
        Dictionary<Player, int> voteTotals = new();

        return GetVotingResults(votes, voteTotals);
    }

    private static Dictionary<Player, int> GetVotingResults(Dictionary<Player, Player> votes, Dictionary<Player, int> voteTotals)
    {
        // TODO: This will probably need to be revisited to support the Hunter / Bodyguard

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