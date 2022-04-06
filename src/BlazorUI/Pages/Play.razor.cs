using BlazorUI.Engine;

namespace BlazorUI.Pages;

public partial class Play
{
    public FiveInLine Game {get;} = new FiveInLine();

    private void OnSelectCell(Position p)
    {
        Game.SelectCell(p);
    }

    private Position GetPosition(int x, int y)
    {
        return new Position{X = x, Y = y};        
    }
}