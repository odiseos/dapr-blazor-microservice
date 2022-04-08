
using Microsoft.AspNetCore.Components;
using BlazorUI.Engines;

namespace BlazorUI.Comps;

public partial class Cell
{
    [Parameter]
    public CellModel Ball {get; set;} = new CellModel();

    [Parameter]
    public EventCallback<Position> OnClickCallback { get; set; }

    [Parameter]
    public Position Position {get; set;} = new Position();

    private string cssSelected() => Ball.Selected ? "selected" : string.Empty;

    private async Task CallOnClickCallback()
    {
        if(OnClickCallback.HasDelegate)
        {
            await  OnClickCallback.InvokeAsync(Position);
        }
    }
}