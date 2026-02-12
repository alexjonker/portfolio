using portofolio.Components;
using portofolio.Models;
using portofolio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("portfolio.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<PortfolioConfig>(builder.Configuration);
builder.Services.AddSingleton<PortfolioDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStatusCodePagesWithReExecute("/404", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
