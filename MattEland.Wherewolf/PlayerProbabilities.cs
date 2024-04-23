namespace MattEland.Wherewolf;

public class PlayerProbabilities
{
    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _slotRoleProbabilities = new();

    public void RegisterSlotRoleProbabilities(GameSlot slot, string role, double support, double population)
    {
        if (!_slotRoleProbabilities.TryGetValue(slot, out SlotRoleProbabilities? probabilities))
        {
            probabilities = new SlotRoleProbabilities();
            _slotRoleProbabilities[slot] = probabilities;
        }

        probabilities.SetProbability(role, support, population);
    }

    public SlotRoleProbabilities GetSlotProbabilities(GameSlot slot) 
        => _slotRoleProbabilities[slot];
}