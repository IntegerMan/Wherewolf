using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Messages;

public record SetupRoleChangedMessage(int Index, GameRole? Role)
{
}