namespace MattEland.Wherewolf.Controllers;

public abstract class PlayerController
{
    public abstract string SelectLoneWolfCenterCard(string[] centerSlotNames);

    public abstract string SelectRobberTarget(string[] otherPlayerNames);
}