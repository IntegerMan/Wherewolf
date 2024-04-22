namespace MattEland.Wherewolf;

public class PlayerProbabilities
{
    private readonly PlayerState _playerState;

    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _slotRoleProbabilities = new();

    public PlayerProbabilities(PlayerState playerState)
    {
        _playerState = playerState;
    }

    public void RegisterSlotRoleProbabilities(GameSlot slot, bool isStarting, string role, double support, double population)
    {
        if (!_slotRoleProbabilities.ContainsKey(slot))
        {
            _slotRoleProbabilities[slot] = new SlotRoleProbabilities();
        }

        if (isStarting)
        {
            _slotRoleProbabilities[slot].SetStartRoleProbabilities(role, support, population);
        }
        else
        {
            _slotRoleProbabilities[slot].SetCurrentRoleProbabilities(role, support, population);
        }
    }

    public SlotRoleProbabilities GetSlotProbabilities(GameSlot slot) 
        => _slotRoleProbabilities[slot];
}