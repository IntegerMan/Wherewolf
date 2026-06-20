using MattEland.Wherewolf.Events.Social;

namespace MattEland.Wherewolf.Tests.Probability;

public class ClaimVoteVictoryProbabilityTests
{
    [Fact]
    public void RobberTruthfulClaimShouldReduceRobbedPlayersVoteAgainstRobber()
    {
        // Arrange: A=Robber robs B=Villager; C=Werewolf claims Villager to stay hidden.
        // A claims truthfully via ClaimStartingRoleStrategy (RobberRobbedClaim(A, B, Villager)).
        // B can self-verify the claim (knows their own start role was Villager).
        GameSetup setup = new(new NonShuffler());
        Player pA = new("A", new FixedSelectionController(new ClaimStartingRoleStrategy(), "B"));
        Player pB = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player pC = new("C", new RandomController(new ClaimFixedRoleStrategy(GameRole.Villager, (p, _) => new VillagerNoActionClaim(p))));
        setup.AddPlayers(pA, pB, pC);
        setup.AddRoles(GameRole.Robber, GameRole.Villager, GameRole.Werewolf,
            GameRole.Villager, GameRole.Villager, GameRole.Villager);

        GameManager game = new(setup);
        game.RunToVoting();
        GameState state = game.CurrentState;

        // Verify the claim was made as expected
        RobberRobbedClaim? robbedClaim = state.Claims.OfType<RobberRobbedClaim>().SingleOrDefault();
        robbedClaim.ShouldNotBeNull();
        robbedClaim!.Target.ShouldBe(pB);
        robbedClaim.StolenRole.ShouldBe(GameRole.Villager);

        // Act: compute B's win probabilities for each vote target
        Dictionary<Player, double> probs = VotingHelper.GetVoteVictoryProbabilities(pB, state);

        // Assert: B should not see A as the best vote target — A's truthful claim is verifiable
        // so B's filtered worlds show C (Werewolf) as the real threat.
        probs[pA].ShouldBeLessThan(probs[pC]);
    }

    [Fact]
    public void RobberFalseClaimAboutRobbedPlayerShouldNotReduceVoteAgainstRobber()
    {
        // Arrange: A=Robber robs B=Villager; A lies — claims they got Werewolf from B.
        // B can self-verify this claim as FALSE (B knows they started as Villager, not Werewolf).
        // Hard filter removes worlds consistent with A's lie, leaving A's worlds unchanged
        // (the true world where B=Villager was never eliminated by B's own events anyway).
        // Net effect: B's belief is no better than the no-claim baseline, so A remains
        // equally suspicious vs. C.
        GameSetup setup = new(new NonShuffler());
        Player pA = new("A", new FixedSelectionController(
            new ClaimFixedRoleStrategy(GameRole.Robber, (p, s) =>
                new RobberRobbedClaim(p, s.Players.Single(x => x.Name == "B"), GameRole.Werewolf)),
            "B"));
        Player pB = new("B", new RandomController(new ClaimStartingRoleStrategy()));
        Player pC = new("C", new RandomController(new ClaimFixedRoleStrategy(GameRole.Villager, (p, _) => new VillagerNoActionClaim(p))));
        setup.AddPlayers(pA, pB, pC);
        setup.AddRoles(GameRole.Robber, GameRole.Villager, GameRole.Werewolf,
            GameRole.Villager, GameRole.Villager, GameRole.Villager);

        GameManager game = new(setup);
        game.RunToVoting();
        GameState state = game.CurrentState;

        // Verify A made the false claim
        RobberRobbedClaim? robbedClaim = state.Claims.OfType<RobberRobbedClaim>().SingleOrDefault();
        robbedClaim.ShouldNotBeNull();
        robbedClaim!.StolenRole.ShouldBe(GameRole.Werewolf);

        // Act
        Dictionary<Player, double> probs = VotingHelper.GetVoteVictoryProbabilities(pB, state);

        // Assert: A's false claim gives B no new information advantage — A should NOT have
        // lower win probability than in the truthful-claim case. Concretely, A should remain
        // at least as suspicious as C from B's perspective (no reduction).
        probs[pA].ShouldBeGreaterThanOrEqualTo(probs[pC]);
    }
    [Fact]
    public void WerewolfClaimShouldBeMoreProbableVictoryThanVillagerClaim()
    {
        // Arrange
        GameSetup setup = new(new NonShuffler());
        Player pPerspective = new Player("A", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Villager, (p,_) => new VillagerNoActionClaim(p))));
        Player pClaimsWolf = new Player("B", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Werewolf, (p,s) => new LoneWerewolfClaim(p, s.CenterSlots.First(), GameRole.Werewolf))));
        Player pClaimsVillager = new Player("C", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Villager, (p,_) => new VillagerNoActionClaim(p))));
        setup.AddPlayers(pPerspective, pClaimsWolf, pClaimsVillager);
        
        // 4 villagers and 2 werewolves, but WWs will stay in the center. We just don't want any uncertainty on role movement so this stays simple
        setup.AddRoles(GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Werewolf, GameRole.Werewolf);
        GameManager game = new(setup);
        game.RunToVoting();
        GameState state = game.CurrentState;

        // Act
        Dictionary<Player, double> probs = VotingHelper.GetVoteVictoryProbabilities(pPerspective, state);

        // Assert
        state.Claims.Count(c => c.Player == pPerspective).ShouldBe(2); // initial and specific
        state.Claims.Count(c => c.Player == pClaimsVillager).ShouldBe(2); // initial and specific
        state.Claims.Count(c => c.Player == pClaimsWolf).ShouldBe(2); // initial and specific
        state.Claims.OfType<StartRoleClaimedEvent>().Count(c => c.ClaimedRole == GameRole.Villager).ShouldBe(2);
        state.Claims.OfType<StartRoleClaimedEvent>().Count(c => c.ClaimedRole == GameRole.Werewolf).ShouldBe(1);
        probs[pClaimsWolf].ShouldBeLessThan(1);
        probs[pClaimsVillager].ShouldBeLessThan(1);
        probs[pClaimsVillager].ShouldBeLessThan(probs[pClaimsWolf]);
    }
}