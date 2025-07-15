using System.Collections.Generic;

namespace Dockbox.Models
{
    public class DockboxConfig
    {
        public List<DockerImage> Images { get; set; }
        public List<DockerNetwork> Networks { get; set; }
        public List<DockerVolume> Volumes { get; set; }
        public List<DockerContainer> Containers { get; set; }
    }

    public class DockerImage
    {
        /// <summary>
        /// Full image name (e.g., nginx:latest or myregistry.com/app:1.0)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional flag: pull only if not present (default true)
        /// </summary>
        public bool PullIfMissing { get; set; } = true;

        /// <summary>
        /// Optional username (for private registry)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Optional password (for private registry)
        /// </summary>
        public string Password { get; set; }
    }

    public class DockerNetwork
    {
        public string Name { get; set; }
        public string Driver { get; set; } = "bridge";
    }

    public class DockerVolume
    {
        /// <summary>
        /// For named volumes: volume name.
        /// For host mounts: absolute path.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// "named" or "host"
        /// </summary>
        public string Type { get; set; } = "named";
    }

   public class DockerContainer
{
    public string Name { get; set; }
    public string Image { get; set; }
    public string? Command { get; set; }
    public string? Network { get; set; }
    public string? RestartPolicy { get; set; }
    public List<string>? Ports { get; set; }
    public Dictionary<string, string>? Environment { get; set; }
    public List<DockerVolumeMount>? Volumes { get; set; }
    public DockerHealthCheck? HealthCheck { get; set; }  // <-- Add this
}


  public class DockerVolumeMount
    {
        public string Source { get; set; } = "";
        public string Target { get; set; } = "";
        public bool ReadOnly { get; set; } = false;
    }
    public class DockerHealthCheck
{
    public List<string> Test { get; set; }
    public string Interval { get; set; } = "10s";
    public string Timeout { get; set; } = "5s";
    public int? Retries { get; set; } = 3;
}

}
