using System;
using System.Collections.Generic;
using System.IO;
using Dockbox.Models;
using Newtonsoft.Json;

namespace Dockbox.Services
{
    public static class JsonConfigLoader
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// Loads all JSON configs from the given directory.
        /// </summary>
        public static List<DockboxConfig> LoadAllFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Directory not found: {directory}");

            var configs = new List<DockboxConfig>();

            foreach (var file in Directory.GetFiles(directory, "*.json"))
            {
                Console.WriteLine($"🔍 Reading file: {file}");

                try
                {
                    var config = LoadFromFile(file);
                    configs.Add(config);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to parse config file: {file}\n{ex.Message}");
                }
            }

            return configs;
        }

        /// <summary>
        /// Loads a single JSON config file.
        /// </summary>
        public static DockboxConfig LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Config file not found: {path}");

            var json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<DockboxConfig>(json, _settings);

            if (config == null)
                throw new Exception($"Failed to deserialize config: {path}");

            return config;
        }
    }
}
