using Portfolio.Models;

namespace Portfolio.Data;

public static class ProjectData
{
    public static readonly Project[] AllProjectsList =
    [
        new("nowa-dash",
            "A Raspberry Pi dashboard made using Arch Linux ARM, Labwc and Quickshell.",
            ["Linux", "Raspberry Pi"]),

        new("tidawnloader",
            "A self-hosted app to download music from tidal. Made with C# and Blazor",
            ["Linux", "C#", "Blazor", "Docker"]),

        new("xrooster",
            "A Flutter app for viewing your school schedule from MyX.",
            ["Flutter", "Dart", "Android"]),

        new("homelab",
            "A Proxmox-based homelab running Docker, self-hosted services and infrastructure for projects and everyday use.",
            ["Proxmox", "Docker", "Linux"]),

        new("portfolio",
            "A portfolio made with C# and Blazor, running on Linux using Docker.",
            ["Linux", "C#", "Blazor", "Docker"]),
    ];
}