using BlazorUI.Clients;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient<GameClient>(
            client => client.BaseAddress = new Uri(builder.Configuration["ApiGatewayUrlExternal"]));

builder.Services.AddHttpClient<UserClient>(
            client => client.BaseAddress = new Uri(builder.Configuration["ApiGatewayUrlExternal"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
