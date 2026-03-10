using Microsoft.Extensions.FileProviders;
using portofolio.Components;
using portofolio.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<PortfolioContentProvider>();
builder.Services.AddSingleton<PortfolioDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStatusCodePagesWithReExecute("/404", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Serve files in the assets folder in content thingies
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.Replace('\\', '/').Trim('/');

    if (path != null && path.Contains("/assets/"))
    {
        var fileInfo = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "content")).GetFileInfo(path);

        if (fileInfo.Exists)
        {
            await context.Response.SendFileAsync(fileInfo);
            return;
        }
    }

    await next();
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
