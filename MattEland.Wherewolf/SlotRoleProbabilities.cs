using System.Collections;

namespace MattEland.Wherewolf;

public class SlotRoleProbabilities : IEnumerable<KeyValuePair<string, double>>
{
    private readonly Dictionary<string, double> _roleProbabilities = new();
    
    public void SetRoleProbabilities(string role, int support, int population)
    {
        if (population <= 0) throw new ArgumentOutOfRangeException(nameof(population)); 
        
        _roleProbabilities[role] = (double)support / population;
    }

    public double this[string role] => _roleProbabilities[role];

    public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
    {
        return _roleProbabilities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_roleProbabilities).GetEnumerator();
}