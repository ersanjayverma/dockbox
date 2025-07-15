using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dockbox.Models;

namespace Dockbox.Services
{
    public static class DockerManager
    {
        public static void CreateNetwork(DockerNetwork network)
        {
            var existing = ShellHelper.Run("docker", $"network ls --filter name=^{network.Name}$ --format \"{{{{.Name}}}}\"").Trim();

            if (existing == network.Name)
            {
                Console.WriteLine($"✔ Network already exists: {network.Name}");
                return;
            }

            Console.WriteLine($"🔧 Creating network: {network.Name}");
            ShellHelper.Run("docker", $"network create --driver {network.Driver} {network.Name}");
        }

        public static void CreateVolumes(List<DockerVolume> volumes)
        {
            foreach (var vol in volumes)
            {
                if (vol.Type == "named")
                {
                    Console.WriteLine($"📦 Creating named volume: {vol.Name}");
                    ShellHelper.Run("docker", $"volume create {vol.Name}");
                }
                else if (vol.Type == "host")
                {
                    Console.WriteLine($"📂 Ensuring host path exists: {vol.Name}");
                    Directory.CreateDirectory(vol.Name);
                }
            }
        }

        public static void RunContainer(DockerContainer container)
        {
            Console.WriteLine($"🚀 Starting container: {container.Name}");

            var cmd = $"run -d --name {container.Name}";

            if (!string.IsNullOrWhiteSpace(container.Network))
                cmd += $" --network {container.Network}";

            if (!string.IsNullOrWhiteSpace(container.RestartPolicy))
                cmd += $" --restart {container.RestartPolicy}";

            if (container.Environment != null)
            {
                foreach (var env in container.Environment)
                    cmd += $" -e \"{env.Key}={env.Value}\"";
            }

            if (container.Ports != null)
            {
                foreach (var port in container.Ports)
                    cmd += $" -p {port}";
            }

            if (container.Volumes != null)
            {
                foreach (var vol in container.Volumes)
                {
                    var mode = vol.ReadOnly ? ":ro" : "";
                    cmd += $" -v {vol.Source}:{vol.Target}{mode}";
                }
            }

            if (container.HealthCheck != null && container.HealthCheck.Test?.Count > 0)
            {
                string testCmd = string.Join(" ", container.HealthCheck.Test);
                cmd += $" --health-cmd=\"{testCmd}\"";

                if (!string.IsNullOrWhiteSpace(container.HealthCheck.Interval))
                    cmd += $" --health-interval={container.HealthCheck.Interval}";

                if (!string.IsNullOrWhiteSpace(container.HealthCheck.Timeout))
                    cmd += $" --health-timeout={container.HealthCheck.Timeout}";

                if (container.HealthCheck.Retries.HasValue)
                    cmd += $" --health-retries={container.HealthCheck.Retries.Value}";
            }

            cmd += $" {container.Image}";

            if (!string.IsNullOrWhiteSpace(container.Command))
                cmd += $" {container.Command}";

            Console.WriteLine($"🐳 Final docker command: docker {cmd}");
            ShellHelper.Run("docker", cmd);
        }
    }
}
