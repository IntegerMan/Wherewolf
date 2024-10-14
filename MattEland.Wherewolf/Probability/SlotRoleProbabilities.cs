using System.Collections;
using System.Collections.ObjectModel;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Probability;

public class SlotRoleProbabilities : IEnumerable<KeyValuePair<GameRole, double>>
{
    private readonly Dictionary<GameRole, double> _probabilities = new();
    
    public void SetProbability(GameRole role, double support, double population)
    {
        if (population <= 0)
        {
            _probabilities[role] = 0;
        }
        else
        {
            _probabilities[role] = support / population;
        }
    }
    
    public ReadOnlyDictionary<GameRole, double> Role => _probabilities.AsReadOnly();
    
    public IEnumerator<KeyValuePair<GameRole, double>> GetEnumerator() => _probabilities.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_probabilities).GetEnumerator();

    public double this[GameRole key] => _probabilities[key];
    public int Count => _probabilities.Count;
}