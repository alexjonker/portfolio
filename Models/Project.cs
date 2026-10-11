namespace Portfolio.Models;

public record Project(
    bool Featured,
    string Name,
    string Description,
    string[] Tags,
    ProjectLink[] Links
);

public record ProjectLink(
    string Name,
    string Url
);