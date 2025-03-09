using MattEland.Wherewolf.Events.Social;
using MattEland.Wherewolf.Roles;

namespace MattEland.Wherewolf.BlazorFrontEnd.Messages;

public record SpecificRoleClaimNeededMessage(SpecificRoleClaim[] PossibleClaims, GameRole InitialClaim);