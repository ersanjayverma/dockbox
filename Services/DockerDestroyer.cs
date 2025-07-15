using System;
using System.Collections.Generic;
using Dockbox.Models;

namespace Dockbox.Services
{
    public static class DockerDestroyer
    {
        public static void Destroy(DockboxConfig config)
        {
            foreach (var container in config.Containers)
            {
                Console.WriteLine($"🗑 Removing container: {container.Name}");
                ShellHelper.Run("docker", $"rm -f {container.Name}");
            }

            foreach (var volume in config.Volumes)
            {
                if (volume.Type == "named")
                {
                    Console.WriteLine($"📦 Removing named volume: {volume.Name}");
                    ShellHelper.Run("docker", $"volume rm {volume.Name}");
                }
            }

            foreach (var network in config.Networks)
            {
                Console.WriteLine($"🌐 Removing network: {network.Name}");
                ShellHelper.Run("docker", $"network rm {network.Name}");
            }

            Console.WriteLine("✅ Destroy complete for config.");
        }
    }
}
