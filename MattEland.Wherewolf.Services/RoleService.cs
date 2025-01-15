using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.Services;

public class RoleService
{
    public string GetDescription(GameRole role) 
        => role switch
        {
            GameRole.Villager => "Villagers are on the villager team and do not get any special abilities or additional information.",
            GameRole.Werewolf => "Werewolves are trying not to get caught. They know who each other are. If a werewolf is alone, they get to see a card from the center.",
            GameRole.Insomniac => "The Insomniac wakes up last and gets to see their card.",
            GameRole.Robber => "The Robber gets to swap cards with another player and then look at their new card.",
            _ => $"Unknown role: {role}"
        };
}