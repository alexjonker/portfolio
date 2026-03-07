using portofolio.Models;

namespace portofolio.Services;

public sealed class PortfolioDataService
{
    private readonly PortfolioContentProvider _provider;

    public PortfolioDataService(PortfolioContentProvider provider)
    {
        _provider = provider;
    }

    private PortfolioContent Content => _provider.Current;

    public SiteConfig Site => Content.Site;
    public HomeConfig Home => Content.Home;
    public IReadOnlyList<ProjectItem> Projects => Content.Projects;
    public IReadOnlyList<EducationItem> Education => Content.Education;
    public IReadOnlyList<SkillItem> Skills => Content.Skills;

    // Generic lookup by slug
    private static TItem? GetBySlug<TItem>(IEnumerable<TItem> items, string? slug) where TItem : class
    {
        if (string.IsNullOrWhiteSpace(slug)) return null;

        var property = typeof(TItem).GetProperty("Slug")!;
        return items.FirstOrDefault(item =>
            string.Equals((string?)property.GetValue(item), slug, StringComparison.OrdinalIgnoreCase));
    }

    public ProjectItem? GetProject(string? slug) => GetBySlug(Projects, slug);
    public EducationItem? GetEducation(string? slug) => GetBySlug(Education, slug);
    public SkillItem? GetSkill(string? slug) => GetBySlug(Skills, slug);

    // Generic resolver
    private static IReadOnlyList<TItem> ResolveBySlugs<TItem>(IEnumerable<TItem> items, IEnumerable<string>? slugs) where TItem : class
    {
        if (slugs is null) return Array.Empty<TItem>();

        var map = items.ToDictionary(
            item => (string)typeof(TItem).GetProperty("Slug")!.GetValue(item)!,
            StringComparer.OrdinalIgnoreCase
        );

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resolved = new List<TItem>();

        foreach (var slug in slugs)
        {
            if (string.IsNullOrWhiteSpace(slug) || !seen.Add(slug)) continue;
            if (map.TryGetValue(slug, out var item)) resolved.Add(item);
        }

        return resolved;
    }

    public IReadOnlyList<ProjectItem> ResolveProjects(IEnumerable<string>? slugs) => ResolveBySlugs(Projects, slugs);
    public IReadOnlyList<EducationItem> ResolveEducation(IEnumerable<string>? slugs) => ResolveBySlugs(Education, slugs);
    public IReadOnlyList<SkillItem> ResolveSkills(IEnumerable<string>? slugs) => ResolveBySlugs(Skills, slugs);

    // Generic "home" helper
    private static IReadOnlyList<TItem> GetHomeItems<TItem>(
        IEnumerable<TItem> allItems,
        IEnumerable<string>? pinnedSlugs,
        int fallbackCount,
        Func<IEnumerable<string>?, IReadOnlyList<TItem>> resolver)
    {
        var featured = resolver(pinnedSlugs);
        return featured.Count > 0 ? featured : allItems.Take(fallbackCount).ToList();
    }

    public IReadOnlyList<ProjectItem> GetHomeProjects(int fallbackCount) =>
        GetHomeItems(Projects, Home.Pinned.Projects, fallbackCount, ResolveProjects);

    public IReadOnlyList<EducationItem> GetHomeEducation(int fallbackCount) =>
        GetHomeItems(Education, Home.Pinned.Education, fallbackCount, ResolveEducation);

    public IReadOnlyList<SkillItem> GetHomeSkills(int fallbackCount) =>
        GetHomeItems(Skills, Home.Pinned.Skills, fallbackCount, ResolveSkills);
}
