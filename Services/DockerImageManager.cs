using System;
using Dockbox.Models;

namespace Dockbox.Services
{
    public static class DockerImageManager
    {
        public static void EnsureImage(DockerImage image)
        {
            string check = ShellHelper.Run("docker", $"images -q {image.Name}").Trim();

            if (!string.IsNullOrEmpty(check) && image.PullIfMissing)
            {
                Console.WriteLine($"✔ Image already present: {image.Name}");
                return;
            }

            Console.WriteLine($"⬇ Pulling image: {image.Name}");

            if (!string.IsNullOrEmpty(image.Username) && !string.IsNullOrEmpty(image.Password))
            {
                Console.WriteLine("🔐 Logging into registry...");
                ShellHelper.Run("docker", $"login -u {image.Username} -p {image.Password}");
            }

            ShellHelper.Run("docker", $"pull {image.Name}");
        }
    }
}
