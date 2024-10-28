using System.Collections.Immutable;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameResult(IEnumerable<Player> dead, GameState state, IDictionary<Player, int> votes, int supportingClaims)
{
    public IEnumerable<Player> DeadPlayers { get; } = dead.ToImmutableList();
    public IEnumerable<GameRole> DeadRoles => DeadPlayers.Select(p => state[p.Name].Role).Distinct();
    public int SupportingClaims => supportingClaims;

    public Team WinningTeam
    {
        get
        {
            // TODO: Revisit with minion
            bool evilsArePresent = state.PlayerSlots.Any(p => p.Role.GetTeam() == Team.Werewolf);
            bool evilsAreDead = DeadRoles.Any(r => r.GetTeam() == Team.Werewolf);
            bool anyAreDead = DeadPlayers.Any();

            // If evil is present, evil will win unless at least one evil is executed.
            // If evil is absent, good wins unless they killed someone unnecessarily.
            // TODO: Revisit with Tanner
            bool isTownWin;
            if (anyAreDead)
            {
                isTownWin = evilsAreDead;
            }
            else
            {
                isTownWin = !evilsArePresent;
            }
            
            return isTownWin ? Team.Villager : Team.Werewolf;
        }
    }

    public IEnumerable<Player> WinningPlayers
    {
        get
        {
            Team winningTeam = WinningTeam;
            return state.PlayerSlots.Where(s => s.Role.GetTeam() == winningTeam).Select(s => s.Player!);
        }
    }
    
    public IEnumerable<Player> LosingPlayers
    {
        get
        {
            Team winningTeam = WinningTeam;
            return state.PlayerSlots.Where(s => s.Role.GetTeam() != winningTeam).Select(s => s.Player!);
        }
    }

    public IDictionary<Player, int> Votes => votes;
}