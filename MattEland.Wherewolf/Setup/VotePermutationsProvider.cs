namespace MattEland.Wherewolf.Setup;

public class VotePermutationsProvider
{
    private readonly Player[] _players;
    private readonly IDictionary<Player, Player>[] _perms;

    public VotePermutationsProvider(IEnumerable<Player> players)
    {
        _players = players.ToArray();
        
        _perms = GetVotingPermutations(_players[0], new Dictionary<Player, Player>())
            .Select(p => p)
            .ToArray();
    }
    
    public IDictionary<Player, Player>[] VotingPermutations => _perms;

    private IEnumerable<IDictionary<Player, Player>> GetVotingPermutations(Player player, IDictionary<Player, Player> baseDictionary)
    {
        IEnumerable<Player> playerChoices = _players.Where(p => p != player);

        foreach (var choice in playerChoices)
        {
            Dictionary<Player, Player> subset = new(baseDictionary)
            {
                [player] = choice
            };

            Player? nextPlayer = _players.FirstOrDefault(p => !subset.ContainsKey(p));
            if (nextPlayer == null)
            {
                yield return subset;
            }
            else
            {
                foreach (var result in GetVotingPermutations(nextPlayer, subset))
                {
                    yield return result;
                }
            }
        }
    }
}