using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf;

public class GameResult
{
    private readonly GameState _state;
    private readonly List<Player> _winningPlayers = new();

    public GameResult(IEnumerable<Player> dead, GameState state, IDictionary<Player, int> votes, int supportingClaims)
    {
        _state = state;
        Votes = votes;
        SupportingClaims = supportingClaims;
        DeadPlayers = dead;
        
        WinningTeam = DetermineWinningTeam();
        
        // Calculate these here for ease of lookup later for performance reasons
        foreach (var player in state.Players)
        {
            GameSlot slot = state[player.Name];
            if (slot.Role.GetTeam() == WinningTeam)
            {
                _winningPlayers.Add(player);
            }
        }
    }

    public IEnumerable<Player> DeadPlayers { get; }
    public IEnumerable<GameRole> DeadRoles => DeadPlayers.Select(p => _state[p.Name].Role).Distinct();
    public int SupportingClaims { get; }

    public Team WinningTeam { get; }

    private Team DetermineWinningTeam()
    {
        bool evilsArePresent = _state.PlayerSlots.Any(p => p.Role.GetTeam() == Team.Werewolf);
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

    public IEnumerable<Player> WinningPlayers => _winningPlayers;

    public IEnumerable<Player> LosingPlayers => _state.Players.Except(_winningPlayers);

    public IDictionary<Player, int> Votes { get; }
}