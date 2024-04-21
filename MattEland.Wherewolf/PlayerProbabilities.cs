namespace MattEland.Wherewolf;

public class PlayerProbabilities
{
    private readonly PlayerState _playerState;

    private readonly Dictionary<GameSlot, SlotRoleProbabilities> _slotRoleProbabilities = new();

    public PlayerProbabilities(PlayerState playerState)
    {
        _playerState = playerState;
    }

    public void RegisterSlotRoleProbabilities(GameSlot slot, string role, int support, int population)
    {
        if (!_slotRoleProbabilities.ContainsKey(slot))
        {
            _slotRoleProbabilities[slot] = new SlotRoleProbabilities();
        }
        
        _slotRoleProbabilities[slot].SetStartRoleProbabilities(role, support, population);
    }

    public SlotRoleProbabilities GetSlotProbabilities(GameSlot slot) 
        => _slotRoleProbabilities[slot];
}