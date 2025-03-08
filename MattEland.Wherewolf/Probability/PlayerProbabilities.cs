using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public class PlayerProbabilities
{
    private readonly Dictionary<string, SlotRoleProbabilities> _currentRoleProbabilities = new();
    private readonly Dictionary<string, SlotRoleProbabilities> _startRoleProbabilities = new();

    public void RegisterCurrentRoleProbabilities(GameSlot slot, GameRole role, double support, double population, IEnumerable<Player> supportingClaims)
    {
        if (!_currentRoleProbabilities.TryGetValue(slot.Name, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _currentRoleProbabilities[slot.Name] = probabilities;
        }

        probabilities.SetProbability(role, support, population, supportingClaims);
    }

    public void RegisterStartRoleProbabilities(GameSlot slot, GameRole role, double support, double population, IEnumerable<Player> supportingClaims)
    {
        if (!_startRoleProbabilities.TryGetValue(slot.Name, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _startRoleProbabilities[slot.Name] = probabilities;
        }

        probabilities.SetProbability(role, support, population, supportingClaims);
    }

    public SlotRoleProbabilities GetCurrentProbabilities(GameSlot slot) 
        => _currentRoleProbabilities[slot.Name];

    public SlotRoleProbabilities GetStartProbabilities(GameSlot slot) 
        => _startRoleProbabilities[slot.Name];
}