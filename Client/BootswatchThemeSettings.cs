using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudioElf.Theme.Bootswatch.Client
{
    public class BootswatchThemeSettings
    {
        public const string SETTINGS_KEY = "StudioElf.Theme.Bootswatch";

        public bool Login { get; set; } = true;
        public bool Register { get; set; } = true;
        public bool Search { get; set; } = true;
        public string Mode { get; set; } = "";
        public string AdminWidthFluid { get; set; } = "-";
        public bool AdminRemoveGutter { get; set; } = false;
        public string ContentWidthFluid { get; set; } = "-";
        public bool ContentRemoveGutter { get; set; } = false;

        // The raw JSON backing store
        [JsonIgnore]
        public string Serialized { get; set; } = "{}";

        #region Constructors

        public BootswatchThemeSettings()
        {
            // Default values already set via properties
            Serialized = ToJson();
        }

        public BootswatchThemeSettings(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Serialized = "{}";
                return;
            }

            try
            {
                // Deserialize into THIS instance
                var loaded = JsonSerializer.Deserialize<BootswatchThemeSettings>(json);

                Login = loaded?.Login ?? Login;
                Register = loaded?.Register ?? Register;
                Search = loaded?.Search ?? Search;
                Mode = loaded?.Mode ?? Mode;
                AdminWidthFluid = loaded?.AdminWidthFluid ?? AdminWidthFluid;
                AdminRemoveGutter = loaded?.AdminRemoveGutter ?? AdminRemoveGutter;
                ContentWidthFluid = loaded?.ContentWidthFluid ?? ContentWidthFluid;
                ContentRemoveGutter = loaded?.ContentRemoveGutter ?? ContentRemoveGutter;

                Serialized = json;
            }
            catch
            {
                // fallback to defaults
                Serialized = "{}";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the settings as JSON.
        /// </summary>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this,
                new JsonSerializerOptions { WriteIndented = false });
        }

        /// <summary>
        /// Updates Serialized with the current instance's values.
        /// </summary>
        public void Sync()
        {
            Serialized = ToJson();
        }

        /// <summary>
        /// Helper to construct safely.
        /// </summary>
        public static BootswatchThemeSettings FromJson(string json)
        {
            return new BootswatchThemeSettings(json);
        }

        #endregion
    }
}
