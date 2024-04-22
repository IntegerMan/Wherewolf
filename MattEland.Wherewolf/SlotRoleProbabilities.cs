using System.Collections.ObjectModel;

namespace MattEland.Wherewolf;

public class SlotRoleProbabilities
{
    private readonly Dictionary<string, double> _startProbabilities = new();
    private readonly Dictionary<string, double> _currentProbabilities = new();
    
    public void SetStartRoleProbabilities(string role, double support, double population)
    {
        if (population <= 0)
        {
            _startProbabilities[role] = 0;
        }
        else
        {
            _startProbabilities[role] = support / population;
        }
    }
    
    public void SetCurrentRoleProbabilities(string role, double support, double population)
    {
        if (population <= 0)
        {
            _currentProbabilities[role] = 0;
        }
        else
        {
            _currentProbabilities[role] = support / population;
        }
    }    
    
    public ReadOnlyDictionary<string, double> StartRole => _startProbabilities.AsReadOnly();
    public ReadOnlyDictionary<string, double> CurrentRole => _currentProbabilities.AsReadOnly();
}