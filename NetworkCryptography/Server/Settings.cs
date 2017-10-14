/*
 * Author: Shon Verch
 * File Name: Settings.cs
 * Project Name: NetworkCryptography
 * Creation Date: 10/14/2017
 * Modified Date: 10/14/2017
 * Description: Contains all server settings data.
 */

using System.IO;
using Newtonsoft.Json;

namespace NetworkCryptography.Server
{
    /// <summary>
    /// Contains all server settings data.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Path of the settings file.
        /// </summary>
        public const string FilePath = "Data/Settings.json";

        /// <summary>
        /// Indicates whether the settings file exists.
        /// </summary>
        public static bool Exists => File.Exists(FilePath);

        /// <summary>
        /// The port which the server will run on.
        /// </summary>
        [JsonProperty("port")]
        public int Port { get; set; }

        /// <summary>
        /// Private constructor so that Settings can not be created outside.
        /// </summary>
        private Settings()
        {
            Port = 0;
        }

        /// <summary>
        /// Load the settings file into memory.
        /// </summary>
        /// <returns></returns>
        public static Settings Load()
        {
            return !Exists ? null : JsonConvert.DeserializeObject<Settings>(File.ReadAllText(FilePath));
        }
    }
}
