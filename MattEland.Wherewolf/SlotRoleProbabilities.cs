using System.Collections;
using System.Collections.ObjectModel;

namespace MattEland.Wherewolf;

public class SlotRoleProbabilities : IEnumerable<KeyValuePair<string, double>>
{
    private readonly Dictionary<string, double> _probabilities = new();
    
    public void SetProbability(string role, double support, double population)
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
    
    public ReadOnlyDictionary<string, double> Role => _probabilities.AsReadOnly();
    
    public IEnumerator<KeyValuePair<string, double>> GetEnumerator() => _probabilities.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_probabilities).GetEnumerator();

    public double this[string key] => _probabilities[key];
    public int Count => _probabilities.Count;
}