using System.Collections;
using System.Collections.ObjectModel;

namespace MattEland.Wherewolf;

public class SlotRoleProbabilities
{
    private readonly Dictionary<string, double> _startProbabilities = new();
    private readonly Dictionary<string, double> _currentProbabilities = new();
    
    public void SetStartRoleProbabilities(string role, int support, int population)
    {
        if (population <= 0) throw new ArgumentOutOfRangeException(nameof(population)); 
        
        _startProbabilities[role] = (double)support / population;
    }

    public ReadOnlyDictionary<string, double> StartRole => _startProbabilities.AsReadOnly();
    public ReadOnlyDictionary<string, double> CurrentRole => _currentProbabilities.AsReadOnly();
}