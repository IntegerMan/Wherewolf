using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Phases;
using MattEland.Wherewolf.Setup;

namespace MattEland.Wherewolf.Roles;

public static class RoleExtensions
{
    public static IEnumerable<Type> GetNightPhasesForRole(this GameRole role)
    {
        switch (role)
        {
            case GameRole.Werewolf:
                yield return typeof(WerewolfNightPhase);
                break;
            case GameRole.Robber:
                yield return typeof(RobberNightPhase);
                break;
            case GameRole.Insomniac:
                yield return typeof(InsomniacNightPhase);
                break;
            case GameRole.Villager:
            default:
                yield break;
        } 
    }
    
    public static IEnumerable<SpecificRoleClaim> GetPossibleSpecificRoleClaims(this GameRole role, Player player, GameState state)
    {
        GameRole[] roles = state.Roles.Distinct().ToArray();
        GameSlot[] centerSlots = state.CenterSlots.ToArray();
        Player[] otherPlayers = state.Players.Where(p => p != player).ToArray();
            
        switch (role)
        {
            case GameRole.Werewolf:
                foreach (var slot in centerSlots)
                {
                    foreach (var newRole in roles)
                    {
                        yield return new LoneWerewolfClaim(player, slot, newRole);
                    }
                }
                    
                // TODO: This should eventually support combinatorial claims with > 2 werewolves in a game
                foreach (var otherPlayer in otherPlayers)
                {
                    yield return new WokeWithWerewolvesClaim(player, otherPlayer);
                }
                    
                break;
            case GameRole.Robber:
                foreach (var target in otherPlayers)
                {
                    foreach (var newRole in roles)
                    {
                        yield return new RobberRobbedClaim(player, target, newRole);
                    }
                }
                break;
            case GameRole.Insomniac:
                foreach (var newRole in roles) 
                {
                    yield return new InsomniacWakeClaim(player, newRole);
                }
                break;
            case GameRole.Villager:
                yield return new VillagerNoActionClaim(player);
                break;
            default:
                throw new NotImplementedException();
        }
    }
    
    public static Team GetTeam(this GameRole role)
    {
        return role switch
        {
            GameRole.Werewolf => Team.Werewolf,
            _ => Team.Villager
        };
    }
}