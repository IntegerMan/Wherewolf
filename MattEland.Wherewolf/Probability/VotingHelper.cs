using MattEland.Wherewolf.Events.Game;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public static class VotingHelper
{
    public static Dictionary<Player, double> GetVoteVictoryProbabilities(Player player, GameState state)
    {
        GamePhase? votingPhase = null;
        GameEvent[] observedEvents = state.Events.Where(e => e.IsObservedBy(player)).ToArray();
        IEnumerable<GameState> permutations = state.Setup.GetPermutationsAtPhase(votingPhase)
            .Where(p => p.IsPossibleGivenEvents(observedEvents));
        
        // Set up our results
        Dictionary<Player, VoteVictoryStatistics> results = new(state.Players.Count() - 1);
        foreach (var otherPlayer in state.Players.Where(p => p != player))
        {
            results[otherPlayer] = new();
        }
        
        // Tabulate results based on who the player voted for that permutation
        foreach (var perm in permutations)
        {
            VotedEvent vote = perm.Events.OfType<VotedEvent>().First(v => v.VotingPlayer == player);

            results[vote.TargetPlayer].Support++;
            if (perm.GameResult!.DidPlayerWin(player))
            {
                results[vote.TargetPlayer].Wins++;
            }
        }
        
        // Translate results into probabilities
        Dictionary<Player, double> winProbability = new(results.Count);
        foreach (var kvp in results)
        {
            winProbability[kvp.Key] = kvp.Value.WinPercent;
        }
        
        return winProbability;
    }

    public static IEnumerable<GameState> GetPossibleGameStatesForPlayer(Player player, GameState state)
    {
        GameEvent[] observedEvents = state.Events.Where(e => e.IsObservedBy(player)).ToArray();
        GamePhase? currentPhase = state.CurrentPhase;
        IEnumerable<GameState> states = state.Setup.GetPermutationsAtPhase(currentPhase);
        
        return states.Where(p => p.IsPossibleGivenEvents(observedEvents));
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
        Dictionary<Player, double> probabilities = GetVoteVictoryProbabilities(votingPlayer, world);

        double maxProb = probabilities.Max(kvp => kvp.Value);

        List<KeyValuePair<Player, double>> atMax = probabilities.Where(kvp => kvp.Value >= maxProb).ToList();

        return atMax.Count == 1 
            ? atMax.First().Key 
            : atMax[rand.Next(atMax.Count)].Key;
    }
    
    public static IEnumerable<KeyValuePair<GameRole, VoteStatistics>> GetRoleClaimVoteStatistics(Player player, GameState gameState)
    {
        GameState[] possibleEndStates = gameState.Setup.GetPermutationsAtPhase(null).ToArray();

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<GameRole, VoteStatistics> roleStats 
            = GetOtherPlayersWinRatesGivenMyRoleClaim(player, gameState, possibleEndStates);
        
        return roleStats;
    }
    
    private static Dictionary<GameRole, VoteStatistics> GetOtherPlayersWinRatesGivenMyRoleClaim(
        Player player, 
        GameState gameState, 
        GameState[] possibleEndStates)
    {
        Dictionary<GameRole, VoteStatistics> roleStats = new();

        // NOTE: No Distinct. We want this weighted for double inclusion where appropriate
        IEnumerable<GameRole> roles = gameState.Setup.Roles;
        GameState[] possibleStates = GetPossibleGameStatesForPlayer(player, gameState).ToArray();
        StartRoleClaimedEvent[] priorClaims = gameState.Claims.OfType<StartRoleClaimedEvent>().ToArray();
        
        foreach (var possibleRole in roles)
        {
            roleStats[possibleRole] = new()
            {
                OtherClaims = priorClaims.Count(e => e.ClaimedRole == possibleRole)
            };
            
            // Filter to roles we started as the role we're considering claiming
            GameState[] roleStates = possibleEndStates.Where(s => s.GetStartRole(player) == possibleRole).ToArray();

            foreach (var hypotheticalState in roleStates)
            {
                roleStats[possibleRole].Games++;
                roleStats[possibleRole].Support += hypotheticalState.Support;
                roleStats[possibleRole].VotesReceived += hypotheticalState.Events.OfType<VotedEvent>().Count(ve => ve.TargetPlayer == player);
            }
            
            roleStats[possibleRole].InPlayPercent = possibleStates.Count(s => s.Root.PlayerSlots.Any(p => p.Role == possibleRole)) / (double)possibleStates.Length;
            roleStats[possibleRole].OutOfPlayPercent = possibleStates.Count(s => s.Root.CenterSlots.Any(c => c.Role == possibleRole)) / (double)possibleStates.Length;
        }

        return roleStats;
    }
}