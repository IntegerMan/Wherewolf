using MattEland.Wherewolf.Events.Social;

namespace MattEland.Wherewolf.Tests.Roles;

public class PossibleClaimsTests : RoleTestBase
{
	// ──────────────────────────────────────────────────────────────────────────────
	// Robber claims
	// ──────────────────────────────────────────────────────────────────────────────

	[Fact]
	public void Robber_CannotClaimStolenRobberRole()
	{
		// Arrange: 1 Robber card in game; Player A = Robber
		GameState state = BuildSingleRobberGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert: claiming to have stolen the only Robber card from another player is impossible
		claims.OfType<RobberRobbedClaim>().Any(c => c.StolenRole == GameRole.Robber).ShouldBeFalse();
	}

	[Theory]
	[InlineData(GameRole.Werewolf)]
	[InlineData(GameRole.Villager)]
	[InlineData(GameRole.Insomniac)]
	public void Robber_CanClaimToHaveStolenOtherRole(GameRole stolenRole)
	{
		// Arrange
		GameState state = BuildSingleRobberGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert: each non-Robber role exists in another slot, so these steals are possible
		claims.OfType<RobberRobbedClaim>().Any(c => c.StolenRole == stolenRole).ShouldBeTrue();
	}

	// ──────────────────────────────────────────────────────────────────────────────
	// Insomniac claims
	// ──────────────────────────────────────────────────────────────────────────────

	[Theory]
	[InlineData(GameRole.Werewolf)]
	[InlineData(GameRole.Villager)]
	public void Insomniac_CannotClaimWakingAsRoleUnattainableWithoutSwapMechanism(GameRole impossibleFinalRole)
	{
		// Arrange: Player A = Insomniac. Only the Robber can swap cards (giving the victim
		// the Robber card), so the Insomniac can never end up holding a Werewolf or Villager card.
		GameState state = BuildInsomniacGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<InsomniacWakeClaim>().Any(c => c.FinalRole == impossibleFinalRole).ShouldBeFalse();
	}

	[Theory]
	[InlineData(GameRole.Insomniac)]
	[InlineData(GameRole.Robber)]
	public void Insomniac_CanClaimWakingAsInsomniacOrRobber(GameRole possibleFinalRole)
	{
		// Arrange: Player A = Insomniac. They can stay Insomniac (not robbed) or become
		// Robber (robbed by the Robber player), so both final-role claims are valid.
		GameState state = BuildInsomniacGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<InsomniacWakeClaim>().Any(c => c.FinalRole == possibleFinalRole).ShouldBeTrue();
	}

	// ──────────────────────────────────────────────────────────────────────────────
	// Lone Werewolf claims
	// ──────────────────────────────────────────────────────────────────────────────

	[Fact]
	public void LoneWerewolf_CanClaimObservingWerewolfInCenterWhenTwoWerewolfCardsExist()
	{
		// Arrange: 2 Werewolf cards; Player A is lone Werewolf among player slots;
		// Center 1 holds the second Werewolf card, so the lone-wolf peek can reveal it.
		GameState state = BuildTwoWerewolfLoneGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<LoneWerewolfClaim>().Any(c => c.ObservedRole == GameRole.Werewolf).ShouldBeTrue();
	}

	[Fact]
	public void LoneWerewolf_CannotClaimObservingWerewolfInCenterWhenOnlyOneWerewolfCardExists()
	{
		// Arrange: only 1 Werewolf card in the entire game. The lone wolf IS that card,
		// so no center slot can also hold a Werewolf.
		GameState state = BuildOneWerewolfGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<LoneWerewolfClaim>().Any(c => c.ObservedRole == GameRole.Werewolf).ShouldBeFalse();
	}

	// ──────────────────────────────────────────────────────────────────────────────
	// Woke-with-Werewolves claims
	// ──────────────────────────────────────────────────────────────────────────────

	[Fact]
	public void WokeWithWerewolves_CanClaimOnePartnerWhenTwoWerewolfCardsExist()
	{
		// Arrange: 2 Werewolf cards in game; some permutation has both Player A and
		// another player as Werewolves, making the paired-wolf claim coherent.
		GameState state = BuildTwoWerewolfPlayerGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<WokeWithWerewolvesClaim>().ShouldNotBeEmpty();
	}

	[Fact]
	public void WokeWithWerewolves_CannotClaimAnyPartnerWhenOnlyOneWerewolfCardExists()
	{
		// Arrange: only 1 Werewolf card in game. No two players can simultaneously be
		// Werewolves, so waking together with another wolf is impossible.
		GameState state = BuildOneWerewolfGame().RunGame();
		Player playerA = state.Players.First();

		// Act
		SpecificRoleClaim[] claims = state.GeneratePossibleSpecificRoleClaims(playerA);

		// Assert
		claims.OfType<WokeWithWerewolvesClaim>().ShouldBeEmpty();
	}

	// ──────────────────────────────────────────────────────────────────────────────
	// Game setup helpers — NonShuffler assigns roles in declaration order:
	// first three to players A / B / C, last three to center slots 1 / 2 / 3.
	// ──────────────────────────────────────────────────────────────────────────────

	// 1R, 2W, 2V, 1I — the exact setup described in the reported bug
	private static GameSetup BuildSingleRobberGame()
	{
		GameSetup setup = new(new NonShuffler());
		setup.AddPlayers(
			new Player("A", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("B", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
		setup.AddRoles(
			GameRole.Robber,    // Player A
			GameRole.Werewolf,  // Player B
			GameRole.Villager,  // Player C
			GameRole.Werewolf,  // Center 1
			GameRole.Villager,  // Center 2
			GameRole.Insomniac  // Center 3
		);
		return setup;
	}

	// 1I, 1R, 2W, 2V — Player A = Insomniac
	private static GameSetup BuildInsomniacGame()
	{
		GameSetup setup = new(new NonShuffler());
		setup.AddPlayers(
			new Player("A", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("B", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
		setup.AddRoles(
			GameRole.Insomniac, // Player A
			GameRole.Robber,    // Player B
			GameRole.Werewolf,  // Player C
			GameRole.Werewolf,  // Center 1
			GameRole.Villager,  // Center 2
			GameRole.Villager   // Center 3
		);
		return setup;
	}

	// 2W, 2V, 1I, 1R — Player A = lone Werewolf among players; Center 1 = second Werewolf
	private static GameSetup BuildTwoWerewolfLoneGame()
	{
		GameSetup setup = new(new NonShuffler());
		setup.AddPlayers(
			new Player("A", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("B", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
		setup.AddRoles(
			GameRole.Werewolf,  // Player A (lone wolf in player slots)
			GameRole.Villager,  // Player B
			GameRole.Insomniac, // Player C
			GameRole.Werewolf,  // Center 1 — second Werewolf card available for lone-wolf peek
			GameRole.Villager,  // Center 2
			GameRole.Robber     // Center 3
		);
		return setup;
	}

	// 2W, 2V, 1I, 1R — Player A = Werewolf, Player B = Werewolf (paired in actual game)
	private static GameSetup BuildTwoWerewolfPlayerGame()
	{
		GameSetup setup = new(new NonShuffler());
		setup.AddPlayers(
			new Player("A", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("B", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
		setup.AddRoles(
			GameRole.Werewolf,  // Player A
			GameRole.Werewolf,  // Player B
			GameRole.Villager,  // Player C
			GameRole.Villager,  // Center 1
			GameRole.Insomniac, // Center 2
			GameRole.Robber     // Center 3
		);
		return setup;
	}

	// 1W, 2V, 1I, 1R — only one Werewolf card in the entire game
	private static GameSetup BuildOneWerewolfGame()
	{
		GameSetup setup = new(new NonShuffler());
		setup.AddPlayers(
			new Player("A", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("B", new RandomController(new ClaimStartingRoleStrategy())),
			new Player("C", new RandomController(new ClaimStartingRoleStrategy())));
		setup.AddRoles(
			GameRole.Werewolf,  // Player A (only Werewolf in game)
			GameRole.Villager,  // Player B
			GameRole.Insomniac, // Player C
			GameRole.Villager,  // Center 1
			GameRole.Villager,  // Center 2
			GameRole.Robber     // Center 3
		);
		return setup;
	}
}
