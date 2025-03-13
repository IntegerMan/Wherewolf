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
        IEnumerable<GameState> possiblyTrueStates = state.Setup.GetPermutationsAtPhase(votingPhase)
            .Where(p => p.IsPossibleGivenEvents(observedEvents));

        SocialEvent[] otherClaims = state.Claims.Where(c => c.Player != player).ToArray();
        
        // Set up our results
        Dictionary<Player, VoteVictoryStatistics> results = new(state.Players.Count() - 1);
        foreach (var otherPlayer in state.Players.Where(p => p != player))
        {
            results[otherPlayer] = new();
        }
        
        // Tabulate results based on who the player voted for that permutation

        foreach (var possibleState in possiblyTrueStates)
        {
            VotedEvent vote = possibleState.Events.OfType<VotedEvent>().First(v => v.VotingPlayer == player);

            // Support always goes up by one, but we'll also factor in other player's claims as well
            double stateImpact = 1 + possibleState.Support + otherClaims.Count(c => c.IsClaimValidFor(possibleState));;
            results[vote.TargetPlayer].Support += stateImpact;
            
            if (possibleState.GameResult!.DidPlayerWin(player))
            {
                results[vote.TargetPlayer].WinningSupport += stateImpact; // if we don't add the support factor in, the % gets wonky
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
    
    // NOTE: Performance critical!
    public static IEnumerable<KeyValuePair<GameRole, VoteStatistics>> GetRoleClaimVoteStatistics(Player player, GameState gameState)
    {
        GameState[] possibleEndStates = gameState.Setup.GetPermutationsAtPhase(null).ToArray();

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<GameRole, VoteStatistics> roleStats = new();

        // NOTE: No Distinct. We want this weighted for double inclusion where appropriate
        GameRole[] roles = gameState.Setup.Roles.Distinct().ToArray();
        GameState[] possibleStates = GetPossibleGameStatesForPlayer(player, gameState).ToArray();
        StartRoleClaimedEvent[] priorClaims = gameState.Claims.OfType<StartRoleClaimedEvent>().ToArray();

        for (int index = 0; index < roles.Length; index++)
        {
            var possibleRole = roles[index];
            roleStats[possibleRole] = new()
            {
                OtherClaims = priorClaims.Count(e => e.ClaimedRole == possibleRole)
            };

            // Filter to roles we started as the role we're considering claiming
            GameState[] roleStates = possibleEndStates.Where(s => s.GetStartRole(player) == possibleRole).ToArray();

            for (int i = 0; i < roleStates.Length; i++)
            {
                GameState hypotheticalState = roleStates[i];
                roleStats[possibleRole].Games++;
                roleStats[possibleRole].Support += hypotheticalState.Support;
                roleStats[possibleRole].VotesReceived += hypotheticalState.Events.OfType<VotedEvent>()
                    .Count(ve => ve.TargetPlayer == player);
            }

            roleStats[possibleRole].InPlayPercent =
                possibleStates.Count(s => s.Root.PlayerSlots.Any(p => p.Role == possibleRole)) /
                (double)possibleStates.Length;
            roleStats[possibleRole].OutOfPlayPercent =
                possibleStates.Count(s => s.Root.CenterSlots.Any(c => c.Role == possibleRole)) /
                (double)possibleStates.Length;
        }

        return roleStats;
    }

    public static IEnumerable<KeyValuePair<SpecificRoleClaim, VoteStatistics>> GetSpecificRoleClaimVoteStatistics(Player player, GameState gameState, SpecificRoleClaim[] possibleClaims)
    {
        GameState[] possibleEndStates = gameState.Setup.GetPermutationsAtPhase(null).ToArray();

        // Now, let's take roles into account and see what victory % each other player gets voting for us based on who they think we are
        Dictionary<SpecificRoleClaim, VoteStatistics> roleStats 
            = GetOtherPlayersWinRatesGivenMySpecificRoleClaim(player, gameState, possibleEndStates, possibleClaims);
        
        return roleStats;
    }
    
    private static Dictionary<SpecificRoleClaim, VoteStatistics> GetOtherPlayersWinRatesGivenMySpecificRoleClaim(
        Player player, 
        GameState gameState, 
        GameState[] possibleEndStates,
        SpecificRoleClaim[] possibleClaims)
    {
        Dictionary<SpecificRoleClaim, VoteStatistics> roleStats = new();

        GameState[] possibleStates = GetPossibleGameStatesForPlayer(player, gameState).ToArray();
        StartRoleClaimedEvent[] priorClaims = gameState.Claims.OfType<StartRoleClaimedEvent>().ToArray();
        
        foreach (var possibleClaim in possibleClaims)
        {
            roleStats[possibleClaim] = new()
            {
                OtherClaims = priorClaims.Count(e => e.ClaimedRole == possibleClaim.Role && e.Player != player)
            };
            
            // Filter to roles we started as the role we're considering claiming
            GameState[] claimStates = possibleEndStates.Where(s => possibleClaim.IsClaimValidFor(s)).ToArray();

            foreach (var hypotheticalState in claimStates)
            {
                roleStats[possibleClaim].Games++;
                roleStats[possibleClaim].Support += hypotheticalState.Support;
                roleStats[possibleClaim].VotesReceived += hypotheticalState.Events.OfType<VotedEvent>().Count(ve => ve.TargetPlayer == player);
            }
            
            roleStats[possibleClaim].InPlayPercent = possibleStates.Count(s => s.Root.PlayerSlots.Any(p => p.Role == possibleClaim.Role)) / (double)possibleStates.Length;
            roleStats[possibleClaim].OutOfPlayPercent = possibleStates.Count(s => s.Root.CenterSlots.Any(c => c.Role == possibleClaim.Role)) / (double)possibleStates.Length;
        }

        return roleStats;
    }
}