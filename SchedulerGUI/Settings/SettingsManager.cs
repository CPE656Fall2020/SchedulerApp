using System;
using System.IO;
using Newtonsoft.Json;

namespace SchedulerGUI.Settings
{
    /// <summary>
    /// <see cref="SettingsManager"/> provides runtime methods to access and save parameters for modules that
    /// form the GroupMe Desktop Client UI.
    /// </summary>
    public class SettingsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        /// <param name="settingsFile">The name of the settings file to open.</param>
        public SettingsManager(string settingsFile)
        {
            this.SettingsFile = settingsFile ?? throw new ArgumentNullException(nameof(settingsFile));
            this.LoadSettings();
        }

        /// <summary>
        /// Gets or sets the core settings instance.
        /// </summary>
        public CoreSettings CoreSettings { get; set; } = new CoreSettings();

        private string SettingsFile { get; set; }

        /// <summary>
        /// Reads the configuration file.
        /// </summary>
        public void LoadSettings()
        {
            if (File.Exists(this.SettingsFile))
            {
                string json = File.ReadAllText(this.SettingsFile);
                JsonConvert.PopulateObject(json, this);
            }
        }

        /// <summary>
        /// Saves the configuration file.
        /// </summary>
        public void SaveSettings()
        {
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(this.SettingsFile))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                };

                serializer.Serialize(file, this);
            }
        }
    }
}
