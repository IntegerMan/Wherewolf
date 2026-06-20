using MattEland.Wherewolf.Events.Social;

namespace MattEland.Wherewolf.Tests.Controllers;

public class ClaimSafestRoleStrategyTests
{
    [Fact]
    public void RobberWithConfirmableClaimShouldClaimTrueRobberRole()
    {
        // Arrange: A=Robber robs B=Villager; C=Werewolf.
        // After the night phase, A holds the Villager card (village-aligned).
        // A has a truthful RobberRobbedClaim(A, B, Villager) that B can self-verify.
        // ClaimSafestRoleStrategy should recognise this and prefer the informative true claim.
        GameSetup setup = new(new NonShuffler());
        Player pA = new("A", new FixedSelectionController(new ClaimStartingRoleStrategy(), "B"));
        Player pB = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player pC = new("C", new RandomController(new ClaimStartingRoleStrategy()));
        setup.AddPlayers(pA, pB, pC);
        setup.AddRoles(GameRole.Robber, GameRole.Villager, GameRole.Werewolf,
            GameRole.Villager, GameRole.Villager, GameRole.Villager);

        GameManager game = new(setup);
        game.RunToEndOfNight();
        GameState state = game.CurrentState;

        var strategy = new ClaimSafestRoleStrategy(new Random(42));

        // Act
        GameRole roleClaim = strategy.GetRoleClaim(pA, state);
        SpecificRoleClaim[] possibleClaims = state.GeneratePossibleSpecificRoleClaims(pA);
        SpecificRoleClaim specificClaim = strategy.GetSpecificRoleClaim(pA, state, possibleClaims, roleClaim);

        // Assert: strategy claims Robber (true start role) and picks the truthful confirmable claim
        roleClaim.ShouldBe(GameRole.Robber);
        specificClaim.ShouldBeOfType<RobberRobbedClaim>();
        var robbedClaim = (RobberRobbedClaim)specificClaim;
        robbedClaim.Target.ShouldBe(pB);
        robbedClaim.StolenRole.ShouldBe(GameRole.Villager);
    }

    [Fact]
    public void RobberWhoStoleWerewolfShouldNotBeCompelledToConfirmClaim()
    {
        // Arrange: A=Robber robs C=Werewolf; B=Villager is uninvolved.
        // After the night phase, A holds the Werewolf card (werewolf-aligned).
        // Team-gating must prevent ClaimSafestRoleStrategy from surfacing the self-outing claim.
        GameSetup setup = new(new NonShuffler());
        Player pA = new("A", new FixedSelectionController(new ClaimStartingRoleStrategy(), "C"));
        Player pB = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player pC = new("C", new RandomController(new ClaimStartingRoleStrategy()));
        setup.AddPlayers(pA, pB, pC);
        setup.AddRoles(GameRole.Robber, GameRole.Villager, GameRole.Werewolf,
            GameRole.Villager, GameRole.Villager, GameRole.Villager);

        GameManager game = new(setup);
        game.RunToEndOfNight();
        GameState state = game.CurrentState;

        var strategy = new ClaimSafestRoleStrategy(new Random(42));

        // Act
        GameRole roleClaim = strategy.GetRoleClaim(pA, state);
        SpecificRoleClaim[] possibleClaims = state.GeneratePossibleSpecificRoleClaims(pA);
        SpecificRoleClaim specificClaim = strategy.GetSpecificRoleClaim(pA, state, possibleClaims, roleClaim);

        // Assert: the specific claim returned should NOT be the confirming RobberRobbedClaim
        // that reveals A stole Werewolf from C (which would out A as the new Werewolf).
        bool isOuting = specificClaim is RobberRobbedClaim rrc
            && rrc.Target == pC
            && rrc.StolenRole == GameRole.Werewolf;
        isOuting.ShouldBeFalse();
    }
}
