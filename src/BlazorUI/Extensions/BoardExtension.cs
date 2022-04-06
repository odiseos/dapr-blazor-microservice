using BlazorUI.Engine;

namespace BlazorUI.Extension;

public static class BoardExtension
{
    public static CellModel Get(this CellModel[,] b, Position p)
    {
        return b[p.X, p.Y];
    }
}