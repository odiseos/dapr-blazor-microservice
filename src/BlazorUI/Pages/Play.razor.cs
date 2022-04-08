using BlazorUI.Engines;

namespace BlazorUI.Pages;

public partial class Play
{  
    public string UserName { get; set; } = "Anonymous";
    public FiveInLine Game {get;} = new FiveInLine();

    private void OnSelectCell(Position p)
    {
        var gameOver = Game.IsGameOver();
        if (!gameOver)
        {
            Game.SelectCell(p);
            if (Game.IsGameOver())
            {
                Task.WhenAll(_gameClient.PostAsync(new Clients.GameMessage { Points = Game.Points, UserName = UserName }));
            }
        }
    }

    private Position GetPosition(int x, int y)
    {
        return new Position{X = x, Y = y};        
    }
}