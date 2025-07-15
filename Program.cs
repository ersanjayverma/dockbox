using Dockbox.Models;
using Dockbox.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dockbox
{
    class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine("[bold cyan]🧱 Dockbox: Declarative Docker Provisioner[/]");

            bool dryRun = args.Contains("--dry-run");
            bool destroy = args.Contains("--destroy");
            bool apply = args.Contains("--apply");

            string configFolder = "/etc/dockbox";

            // Support: dotnet run --apply myfile.json OR myfile.json --apply
            string? singleConfigFile = args.FirstOrDefault(arg => arg.EndsWith(".json"));
            if (!string.IsNullOrWhiteSpace(singleConfigFile) && !File.Exists(singleConfigFile))
            {
                AnsiConsole.MarkupLine($"[red]❌ Error:[/] Config file not found: {singleConfigFile}");
                return;
            }

            try
            {
                List<DockboxConfig> configs;

                if (!string.IsNullOrWhiteSpace(singleConfigFile))
                {
                    AnsiConsole.MarkupLine($"[blue]🔍 Reading file:[/] {singleConfigFile}");
                    var config = JsonConfigLoader.LoadFromFile(singleConfigFile);
                    configs = new List<DockboxConfig> { config };
                }
                else
                {
                    configs = JsonConfigLoader.LoadAllFromDirectory(configFolder);
                    AnsiConsole.MarkupLine($"[green]✔ Loaded all configs from:[/] {configFolder}");
                }

                foreach (var config in configs)
                    CheckForDuplicates(config);

                foreach (var config in configs)
                    ShowSummaryTable(config);

                if (dryRun)
                {
                    AnsiConsole.MarkupLine("[yellow]🟡 Dry run mode enabled. No changes applied.[/]");
                    return;
                }

                if (destroy)
                {
                    AnsiConsole.MarkupLine("[red]💣 Destroy mode enabled. Cleaning up all defined resources.[/]");
                    foreach (var config in configs)
                        DockerDestroyer.Destroy(config);
                    return;
                }

                if (apply)
                {
                    foreach (var config in configs)
                    {
                        if (config.Images != null)
                            foreach (var image in config.Images)
                                DockerImageManager.EnsureImage(image);

                        if (config.Networks != null)
                            foreach (var net in config.Networks)
                                DockerManager.CreateNetwork(net);

                        if (config.Volumes != null)
                            DockerManager.CreateVolumes(config.Volumes);

                        if (config.Containers != null)
                            foreach (var container in config.Containers)
                                DockerManager.RunContainer(container);
                    }

                    AnsiConsole.MarkupLine("[bold green]🚀 Provisioning complete![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]ℹ️ Nothing applied. Use '--apply' to deploy or '--destroy' to clean up.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]❌ Error:[/] {ex.Message}");
            }
        }

        static void CheckForDuplicates(DockboxConfig config)
        {
            var duplicateContainers = config.Containers?
                .GroupBy(c => c.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            var duplicateVolumes = config.Volumes?
                .GroupBy(v => v.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateContainers?.Any() == true)
                throw new Exception($"Duplicate container names found: {string.Join(", ", duplicateContainers)}");

            if (duplicateVolumes?.Any() == true)
                throw new Exception($"Duplicate volume names found: {string.Join(", ", duplicateVolumes)}");
        }

        static void ShowSummaryTable(DockboxConfig config)
        {
            var table = new Table().Title("📦 Dockbox Provisioning Plan").RoundedBorder().Expand();

            table.AddColumn("[blue]Type[/]");
            table.AddColumn("[green]Name[/]");
            table.AddColumn("[yellow]Details[/]");

            if (config.Images != null)
                foreach (var img in config.Images)
                    table.AddRow("Image", img.Name, img.PullIfMissing ? "Pull if missing" : "Skip");

            if (config.Networks != null)
                foreach (var net in config.Networks)
                    table.AddRow("Network", net.Name, net.Driver);

            if (config.Volumes != null)
                foreach (var vol in config.Volumes)
                    table.AddRow("Volume", vol.Name, vol.Type);

            if (config.Containers != null)
                foreach (var c in config.Containers)
                    table.AddRow("Container", c.Name, c.Image);

            AnsiConsole.Write(table);
        }
    }
}
