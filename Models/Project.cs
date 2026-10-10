namespace Portfolio.Models;

public record Project(
    string Name,
    string Description,
    string[] Tags
);