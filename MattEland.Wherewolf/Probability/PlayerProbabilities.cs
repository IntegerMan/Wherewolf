using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public class PlayerProbabilities
{
    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _currentRoleProbabilities = new();
    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _startRoleProbabilities = new();

    public void RegisterCurrentRoleProbabilities(GameSlot slot, GameRole role, double support, double population)
    {
        if (!_currentRoleProbabilities.TryGetValue(slot, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _currentRoleProbabilities[slot] = probabilities;
        }

        probabilities.SetProbability(role, support, population, Enumerable.Empty<Player>());
    }

    public void RegisterStartRoleProbabilities(GameSlot slot, GameRole role, double support, double population)
    {
        if (!_startRoleProbabilities.TryGetValue(slot, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _startRoleProbabilities[slot] = probabilities;
        }

        probabilities.SetProbability(role, support, population, Enumerable.Empty<Player>());
    }
    

    public SlotRoleProbabilities GetCurrentProbabilities(GameSlot slot) 
        => _currentRoleProbabilities[slot];

    public SlotRoleProbabilities GetStartProbabilities(GameSlot slot) 
        => _startRoleProbabilities[slot];
    
}