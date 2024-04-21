namespace MattEland.Wherewolf.Controllers;

public class FixedSelectionController : PlayerController
{
    private readonly string _selection;
    
    public FixedSelectionController(string selection)
    {
        _selection = selection;
    }
    
    public override string SelectLoneWolfCenterCard(string[] centerSlotNames) 
        => centerSlotNames.Single(s => s == _selection);
}