using System.Linq;
using BenchmarkDotNet.Attributes;
using MattEland.Wherewolf.Controllers;
using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Probability;
using MattEland.Wherewolf.Roles;
using MattEland.Wherewolf.Setup;
using Microsoft.VSDiagnostics;

namespace MattEland.Wherewolf.Benchmarks;
[CPUUsageDiagnoser]
public class CalculateProbabilitiesBenchmarks
{
    private GameState _state = null!;
    private Player _perspective = null!;
    [GlobalSetup]
    public void Setup()
    {
        GameSetup setup = new(new NonShuffler());
        Player pPerspective = new Player("A", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Villager, (p, _) => new VillagerNoActionClaim(p))));
        Player pClaimsWolf = new Player("B", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Werewolf, (p, s) => new LoneWerewolfClaim(p, s.CenterSlots.First(), GameRole.Werewolf))));
        Player pClaimsVillager = new Player("C", new FixedSelectionController(new ClaimFixedRoleStrategy(GameRole.Villager, (p, _) => new VillagerNoActionClaim(p))));
        setup.AddPlayers(pPerspective, pClaimsWolf, pClaimsVillager);
        setup.AddRoles(GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Villager, GameRole.Werewolf, GameRole.Werewolf);
        GameManager game = new(setup);
        game.RunToVoting();
        _state = game.CurrentState;
        _perspective = pPerspective;
    }

    [Benchmark]
    public PlayerProbabilities CalculateProbabilities() => _state.CalculateProbabilities(_perspective);
}