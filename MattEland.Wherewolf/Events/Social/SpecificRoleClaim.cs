using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Events.Social;

/// <summary>
/// Represents a specific role claim made by a player. For example, a player claiming they were an insomniac who saw
/// they were still an insomniac, or a player claiming to be the robber who stole the villager from player X.
/// </summary>
public abstract class SpecificRoleClaim(Player player, GameRole role) : SocialEvent(player)
{
    public GameRole Role => role;
    public override Team? AssociatedTeam => Role.GetTeam();

    /// <summary>
    /// Cheap combinatorial pre-check based on role-card counts alone. Returns <c>false</c> only
    /// when the claim is provably impossible regardless of role assignments (e.g. claiming to
    /// have stolen the Robber role when only one Robber card exists). A return of <c>true</c>
    /// means the claim is potentially valid — the rigorous <see cref="IsClaimValidFor"/> sweep
    /// across all permutations confirms whether it actually holds in at least one game state.
    /// </summary>
    public virtual bool IsCombinatoriallyPossible(GameState state) => true;
    
    public bool? EvaluateTruthfulness(GameState game, Player? perspective)
    {
        if (perspective == null || game.IsGameOver || perspective == Player)
        {
            return IsClaimValidFor(game);
        }

        return null;
    }

    /// <summary>
    /// Returns true when the perspective player's own certain knowledge confirms this claim,
    /// false when their knowledge contradicts it, or null when this claim says nothing
    /// self-verifiable about that player.
    /// </summary>
    public virtual bool? SelfVerify(GameState state, Player perspective) => null;

    /// <summary>
    /// Returns true when at least one other player can independently confirm this claim
    /// based solely on their own certain start-role knowledge.
    /// </summary>
    public bool IsConfirmableByOtherPlayer(GameState state)
        => state.Players.Any(p => p.Name != Player.Name && SelfVerify(state, p) == true);
}