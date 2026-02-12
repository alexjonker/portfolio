using System.Collections.Generic;

namespace portofolio.Models;

public sealed class PortfolioConfig
{
    public List<ProjectItem> Projects { get; set; } = new();
    public List<EducationItem> Education { get; set; } = new();
    public List<SkillItem> Skills { get; set; } = new();
}

public sealed class ProjectItem
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public List<string> skills { get; set; } = new();
    public List<LinkItem> Links { get; set; } = new();
}

public sealed class EducationItem
{
    public string Slug { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> skills { get; set; } = new();
    public List<LinkItem> Links { get; set; } = new();
}

public sealed class SkillItem
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class LinkItem
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
