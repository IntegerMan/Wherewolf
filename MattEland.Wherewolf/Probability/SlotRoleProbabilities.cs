using System.Collections;
using System.Collections.ObjectModel;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public class SlotRoleProbabilities : IEnumerable<KeyValuePair<GameRole, SlotProbability>>
{
    private readonly Dictionary<GameRole, SlotProbability> _probabilities = new();
    
    public void SetProbability(GameRole role, double support, double population, IEnumerable<Player> supportingClaims)
    {
        if (population <= 0)
        {
            _probabilities[role] = new SlotProbability(0, supportingClaims);
        }
        else
        {
            _probabilities[role] = new SlotProbability(support / population, supportingClaims);
        }
    }
    
    public ReadOnlyDictionary<GameRole, SlotProbability> Role => _probabilities.AsReadOnly();
    
    public IEnumerator<KeyValuePair<GameRole, SlotProbability>> GetEnumerator() => _probabilities.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_probabilities).GetEnumerator();

    public SlotProbability this[GameRole key] => _probabilities[key];
    public int Count => _probabilities.Count;
}