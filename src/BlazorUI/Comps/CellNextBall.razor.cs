
using Microsoft.AspNetCore.Components;

namespace BlazorUI.Comps;

public partial class CellNextBall
{
    [Parameter]
    public string Ball_t {get; set;} = string.Empty;

    private void CreateBall()
    {
        Ball_t = $"b{Random.Shared.Next(1, 10)}";
    }
}