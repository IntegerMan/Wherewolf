using MattEland.Wherewolf.Events;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

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
    
    public static bool DoesPlayerWinWithBestVotingOptions(Player player, GameState state)
    {
        Dictionary<Player, Player> votes = new();
        Random rand = new();
        foreach (var votingPlayer in state.Players)
        {
            Dictionary<Player, float> probs = GetVoteVictoryProbabilities(votingPlayer, state);
            var bestOptions = probs.Where(kvp => Math.Abs(kvp.Value - probs.Values.Max()) < double.Epsilon).ToList();
            var bestOption = bestOptions.Count == 1 ? bestOptions[0] : bestOptions.MinBy(_ => rand.Next());
            
            votes[votingPlayer] = bestOption.Key;
        }
        
        Dictionary<Player, int> voteTotals = GetVotingResults(votes);
        
        GameResult gameResult = state.DetermineGameResults(voteTotals);
        
        return gameResult.WinningPlayers.Contains(player);
    }    
    
    public static float GetAssumedStartRoleVictoryProbabilities(Player player, GameState state, GameRole role)
    {
        // Identify all starting game states where the player is the role
        List<GameState> permutations = state.Setup.GetSetupPermutations()
                                                  .Where(s => s.GetStartRole(player) == role)
                                                  .ToList();
        
        // Remove ones where other players started as roles the player knows they couldn't have started as
        PlayerProbabilities probabilities = state.CalculateProbabilities(player);
        foreach (var otherPlayer in state.Players.Where(p => p != player))
        {
            foreach (var prob in probabilities.GetStartProbabilities(state.GetPlayerSlot(otherPlayer)))
            {
                if (prob.Value <= 0f)
                {
                    permutations = permutations.Where(s => s.GetStartRole(otherPlayer) != prob.Key).ToList();
                }
            }
        }

        // Given these roles, assume the player voted for the person who gives them the highest win probability (randomize ties)
        int wins = 0;
        foreach (var perm in permutations)
        {
            if (DoesPlayerWinWithBestVotingOptions(player, perm))
            {
                wins++;
            }
        }

        // Return the average win probability for the player
        return wins / (float)permutations.Count;
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

    public static Player GetMostLikelyWinningVote(GameState world, Player votingPlayer, Random randomizer)
    {
        Dictionary<Player, float> probabilities = GetVoteVictoryProbabilities(votingPlayer, world);

        float maxProb = probabilities.Max(kvp => kvp.Value);

        List<KeyValuePair<Player, float>> atMax = probabilities.Where(kvp => kvp.Value >= maxProb).ToList();

        return atMax.Count == 1 
            ? atMax.First().Key 
            : atMax[randomizer.Next(atMax.Count)].Key;
    }
}