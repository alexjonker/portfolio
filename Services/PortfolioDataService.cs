using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using portofolio.Models;

namespace portofolio.Services;

public sealed class PortfolioDataService
{
    private readonly IOptionsMonitor<PortfolioConfig> _options;

    public PortfolioDataService(IOptionsMonitor<PortfolioConfig> options)
    {
        _options = options;
    }

    public IReadOnlyList<ProjectItem> Projects => _options.CurrentValue.Projects;
    public IReadOnlyList<EducationItem> Education => _options.CurrentValue.Education;
    public IReadOnlyList<SkillItem> Skills => _options.CurrentValue.Skills;

    public ProjectItem? GetProject(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        return Projects.FirstOrDefault(project =>
            project.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public EducationItem? GetEducation(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        return Education.FirstOrDefault(item =>
            item.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public SkillItem? GetSkill(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        return Skills.FirstOrDefault(item =>
            item.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<SkillItem> ResolveSkills(IEnumerable<string>? slugs)
    {
        if (slugs is null)
        {
            return Array.Empty<SkillItem>();
        }

        var map = Skills.ToDictionary(skill => skill.Slug, StringComparer.OrdinalIgnoreCase);
        var resolved = new List<SkillItem>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var slug in slugs)
        {
            if (string.IsNullOrWhiteSpace(slug) || !seen.Add(slug))
            {
                continue;
            }

            if (map.TryGetValue(slug, out var skill))
            {
                resolved.Add(skill);
            }
        }

        return resolved;
    }
}
