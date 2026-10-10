namespace Portfolio.Models;

public record Project(
    bool Featured,
    string Name,
    string Description,
    string[] Tags
);