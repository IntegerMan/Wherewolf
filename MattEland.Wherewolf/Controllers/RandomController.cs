namespace MattEland.Wherewolf.Controllers;

public class RandomController : PlayerController
{
    private readonly Random _rand = new();
    
    public override string SelectLoneWolfCenterCard(string[] centerSlotNames) 
        => centerSlotNames.ElementAt(_rand.Next(centerSlotNames.Length));
}