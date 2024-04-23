namespace MattEland.Wherewolf;

public class PlayerProbabilities
{
    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _slotRoleProbabilities = new();

    public void RegisterSlotRoleProbabilities(GameSlot slot, bool isStarting, string role, double support, double population)
    {
        if (!_slotRoleProbabilities.TryGetValue(slot, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _slotRoleProbabilities[slot] = probabilities;
        }

        if (isStarting)
        {
            probabilities.SetStartRoleProbabilities(role, support, population);
        }
        else
        {
            probabilities.SetCurrentRoleProbabilities(role, support, population);
        }
    }

    public SlotRoleProbabilities GetSlotProbabilities(GameSlot slot) 
        => _slotRoleProbabilities[slot];
}