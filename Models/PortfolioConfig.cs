namespace portofolio.Models;

public sealed class PortfolioContent
{
    public string DefaultLanguage { get; set; } = "en";
    public SiteConfig Site { get; set; } = new();
    public HomeConfig Home { get; set; } = new();
    public List<ProjectItem> Projects { get; set; } = new();
    public List<EducationItem> Education { get; set; } = new();
    public List<SkillItem> Skills { get; set; } = new();
}

public sealed class SiteConfig
{
    public string DisplayName { get; set; } = string.Empty;
    public List<SocialLink> Social { get; set; } = new();
}

public sealed class SocialLink
{
    public string IconPath { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public sealed class HomeConfig
{
    public HomeHeroConfig Hero { get; set; } = new();
    public HomePinnedConfig Pinned { get; set; } = new();
}

public sealed class HomeHeroConfig
{
    public string Title { get; set; } = string.Empty;
    public string SummaryHtml { get; set; } = string.Empty;
}

public sealed class HomePinnedConfig
{
    public List<string> Projects { get; set; } = new();
    public List<string> Education { get; set; } = new();
    public List<string> Skills { get; set; } = new();
}

public sealed class ProjectItem
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<LinkItem> Links { get; set; } = new();
}

public sealed class EducationItem
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<LinkItem> Links { get; set; } = new();
}

public sealed class SkillItem
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
}

public sealed class LinkItem
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
