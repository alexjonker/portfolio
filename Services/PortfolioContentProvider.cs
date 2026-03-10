using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using portofolio.Models;

namespace portofolio.Services;

public sealed class PortfolioContentProvider : IDisposable
{
    private readonly ILogger<PortfolioContentProvider> _logger;
    private readonly PhysicalFileProvider _fileProvider;
    private readonly object _sync = new();
    private PortfolioContent _content;

    public PortfolioContentProvider(IWebHostEnvironment env, ILogger<PortfolioContentProvider> logger)
    {
        _logger = logger;
        var contentPath = Path.Combine(env.ContentRootPath, "content");

        _fileProvider = new PhysicalFileProvider(contentPath);
        _content = LoadContent(contentPath);
        WatchForChanges(contentPath);
    }

    public PortfolioContent Current
    {
        get { lock (_sync) return _content; }
    }

    private void WatchForChanges(string path)
    {
        ChangeToken.OnChange(() => _fileProvider.Watch("**/*"), () =>
        {
            try
            {
                var refreshed = LoadContent(path);
                lock (_sync) _content = refreshed;
                _logger.LogInformation("Portfolio content reloaded from {ContentPath}", path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reload portfolio content from {ContentPath}", path);
            }
        });
    }

    private PortfolioContent LoadContent(string root)
    {
        var rootConfig = ReadJson<RootConfig>(Path.Combine(root, "config.json")) ?? new();
        var lang = string.IsNullOrWhiteSpace(rootConfig.DefaultLanguage) ? "en" : rootConfig.DefaultLanguage;
        var home = BuildHomeConfig(root, rootConfig, lang);

        return new PortfolioContent
        {
            DefaultLanguage = lang,
            Site = rootConfig.Site ?? new SiteConfig(),
            Home = home,
            Projects = LoadItems<ProjectConfig, ProjectItem>(
                Path.Combine(root, "projects"), lang, c => new ProjectItem
                {
                    Slug = c.Slug,
                    Name = ResolveLocalized(c.Config.Name, lang),
                    Summary = ResolveLocalized(c.Config.Summary, lang),
                    Html = LoadHtml(c.Folder),
                    Period = c.Config.Period ?? string.Empty,
                    Skills = c.Config.Skills ?? new(),
                    Links = ResolveLinks(c.Config.Links, lang)
                }),
            Education = LoadItems<EducationConfig, EducationItem>(
                Path.Combine(root, "education"), lang, c => new EducationItem
                {
                    Slug = c.Slug,
                    Name = ResolveLocalized(c.Config.Name, lang),
                    Summary = ResolveLocalized(c.Config.Summary, lang),
                    Html = LoadHtml(c.Folder),
                    Period = c.Config.Period ?? string.Empty,
                    Location = ResolveLocalized(c.Config.Location, lang),
                    Skills = c.Config.Skills ?? new(),
                    Links = ResolveLinks(c.Config.Links, lang)
                }),
            Skills = LoadItems<SkillConfig, SkillItem>(
                Path.Combine(root, "skills"), lang, c => new SkillItem
                {
                    Slug = c.Slug,
                    Name = ResolveLocalized(c.Config.Name, lang),
                    Html = LoadHtml(c.Folder)
                })
        };
    }

    private HomeConfig BuildHomeConfig(string root, RootConfig config, string lang) => new()
    {
        Hero = new HomeHeroConfig
        {
            Title = ResolveTitle(config.Home?.Hero, lang),
            SummaryHtml = LoadHtml(Path.Combine(root, "home"), $"{lang}.html", "en.html")
        },
        Pinned = config.Home?.Pinned ?? new()
    };

    private static List<TItem> LoadItems<TConfig, TItem>(
        string path, string lang,
        Func<(string Slug, string Folder, TConfig Config), TItem> map)
        where TConfig : class
    {
        if (!Directory.Exists(path))
            return new();
    
        return Directory.GetDirectories(path)
            .Select(folder => (
                Slug: Path.GetFileName(folder),
                Folder: folder,
                Config: ReadJson<TConfig>(Path.Combine(folder, "config.json"))
            ))
            .Where(x => x.Config != null)
            .Select(x => map((x.Slug, x.Folder, x.Config!))) // <-- add ! here
            .ToList();
    }

    private static string ResolveLocalized(Dictionary<string, string>? values, string lang, string? fallback = null) =>
        values != null && values.TryGetValue(lang, out var val) && !string.IsNullOrWhiteSpace(val)
            ? val
            : values?.Values.FirstOrDefault(static x => !string.IsNullOrWhiteSpace(x)) ?? fallback ?? string.Empty;

    private static string ResolveTitle(HomeHeroRootConfig? hero, string lang)
    {
        if (hero == null) return string.Empty;

        return hero.Title.ValueKind switch
        {
            JsonValueKind.String => hero.Title.GetString() ?? string.Empty,
            JsonValueKind.Object => ResolveLocalized(
                JsonSerializer.Deserialize<Dictionary<string, string>>(hero.Title, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }),
                lang, hero.FallbackTitle),
            _ => hero.FallbackTitle ?? string.Empty
        };
    }

    private static string LoadHtml(string basePath, string? fileName = null, string? fallback = "en.html")
    {
        var path = !string.IsNullOrWhiteSpace(fileName) ? Path.Combine(basePath, fileName) : null;
        if (path != null && File.Exists(path))
            return File.ReadAllText(path);

        path = Path.Combine(basePath, fallback ?? string.Empty);
        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    private static List<LinkItem> ResolveLinks(List<LinkConfig>? links, string lang) =>
        links?.Select(l => new LinkItem
        {
            Label = ResolveLocalized(l.Label, lang),
            Url = l.Url ?? string.Empty
        }).ToList() ?? new();

    private static T? ReadJson<T>(string path)
    {
        if (!File.Exists(path)) return default;
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public void Dispose() => _fileProvider.Dispose();

    // ── Internal config classes ─────────────────────────────────────────────
    private sealed class RootConfig
    {
        public string DefaultLanguage { get; set; } = "en";
        public SiteConfig? Site { get; set; }
        public HomeRootConfig? Home { get; set; }
    }

    private sealed class HomeRootConfig
    {
        public HomeHeroRootConfig? Hero { get; set; }
        public HomePinnedConfig Pinned { get; set; } = new();
    }

    private sealed class HomeHeroRootConfig
    {
        public JsonElement Title { get; set; }
        public string? FallbackTitle { get; set; }
    }

    private sealed class ProjectConfig
    {
        public Dictionary<string, string>? Name { get; set; }
        public Dictionary<string, string>? Summary { get; set; }
        public string? Period { get; set; }
        public List<string>? Skills { get; set; }
        public List<LinkConfig>? Links { get; set; }
        public string? ContentFile { get; set; }
    }

    private sealed class EducationConfig
    {
        public Dictionary<string, string>? Name { get; set; }
        public Dictionary<string, string>? Summary { get; set; }
        public Dictionary<string, string>? Location { get; set; }
        public string? Period { get; set; }
        public List<string>? Skills { get; set; }
        public List<LinkConfig>? Links { get; set; }
        public string? ContentFile { get; set; }
    }

    private sealed class SkillConfig
    {
        public Dictionary<string, string>? Name { get; set; }
        public string? ContentFile { get; set; }
    }

    private sealed class LinkConfig
    {
        public Dictionary<string, string>? Label { get; set; }
        public string? Url { get; set; }
    }
}