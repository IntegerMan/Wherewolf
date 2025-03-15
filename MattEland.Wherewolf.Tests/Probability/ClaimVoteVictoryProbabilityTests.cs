using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;

namespace MattEland.Wherewolf.Tests.Probability;

public class ClaimVoteVictoryProbabilityTests
{
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