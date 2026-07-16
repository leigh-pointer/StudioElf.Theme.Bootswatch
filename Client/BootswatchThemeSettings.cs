using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudioElf.Theme.Bootswatch.Client
{
    public class BootswatchThemeSettings
    {
        public const string SETTINGS_KEY = "StudioElf.Theme.Bootswatch";

        public bool Logo { get; set; } = true;
        public bool Menu { get; set; } = true;
        public bool Login { get; set; } = true;
        public bool Register { get; set; } = true;
        public bool Search { get; set; } = true;
        public bool ShowLanguageSwitcher { get; set; } = false;
        public string Mode { get; set; } = "light";
        public string AdminWidthFluid { get; set; } = "-";
        public bool AdminRemoveGutter { get; set; } = false;
        public string ContentWidthFluid { get; set; } = "-";
        public bool ContentRemoveGutter { get; set; } = false;

        [JsonIgnore]
        public string Serialized { get; private set; } = "{}";

        public BootswatchThemeSettings()
        {
            Serialized = ToJson();
        }

        public static BootswatchThemeSettings FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new BootswatchThemeSettings();
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var obj = JsonSerializer.Deserialize<BootswatchThemeSettings>(json, options);
                if (obj == null) return new BootswatchThemeSettings();
                obj.Serialized = json;
                return obj;
            }
            catch
            {
                return new BootswatchThemeSettings();
            }
        }

        /// <summary>
        /// Merge siteJson and pageJson where pageJson wins when a property exists.
        /// Both inputs are JSON objects (partial or full). Returns a fully populated settings object.
        /// </summary>
        public static BootswatchThemeSettings MergeFromJson(string siteJson, string pageJson)
        {
            var result = new BootswatchThemeSettings();

            try
            {
                if (!string.IsNullOrWhiteSpace(siteJson))
                {
                    using var doc = JsonDocument.Parse(siteJson);
                    var root = doc.RootElement;
                    ApplyJsonTo(result, root);
                }

                if (!string.IsNullOrWhiteSpace(pageJson))
                {
                    using var doc = JsonDocument.Parse(pageJson);
                    var root = doc.RootElement;
                    ApplyJsonTo(result, root);
                }
            }
            catch
            {
                // ignore and return defaults or partially applied
            }

            result.Serialized = ToJson(result);
            return result;
        }

        private static void ApplyJsonTo(BootswatchThemeSettings target, JsonElement root)
        {
            if (root.ValueKind != JsonValueKind.Object) return;

            if (root.TryGetProperty("Logo", out var p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.Logo = p.GetBoolean();

            if (root.TryGetProperty("Menu", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.Menu = p.GetBoolean();

            if (root.TryGetProperty("Login", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.Login = p.GetBoolean();

            if (root.TryGetProperty("Register", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.Register = p.GetBoolean();

            if (root.TryGetProperty("Search", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.Search = p.GetBoolean();

            if (root.TryGetProperty("ShowLanguageSwitcher", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.ShowLanguageSwitcher = p.GetBoolean();

            if (root.TryGetProperty("Mode", out p) && p.ValueKind == JsonValueKind.String)
                target.Mode = p.GetString() ?? target.Mode;

            if (root.TryGetProperty("AdminWidthFluid", out p) && p.ValueKind == JsonValueKind.String)
                target.AdminWidthFluid = p.GetString() ?? target.AdminWidthFluid;

            if (root.TryGetProperty("AdminRemoveGutter", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.AdminRemoveGutter = p.GetBoolean();

            if (root.TryGetProperty("ContentWidthFluid", out p) && p.ValueKind == JsonValueKind.String)
                target.ContentWidthFluid = p.GetString() ?? target.ContentWidthFluid;

            if (root.TryGetProperty("ContentRemoveGutter", out p) && (p.ValueKind == JsonValueKind.True || p.ValueKind == JsonValueKind.False))
                target.ContentRemoveGutter = p.GetBoolean();
        }

        public string ToJson()
        {
            return ToJson(this);
        }

        public static string ToJson(BootswatchThemeSettings obj)
        {
            var options = new JsonSerializerOptions { WriteIndented = false };
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// Helper to detect whether a JSON blob contains a given property at its root.
        /// </summary>
        public static bool JsonHasProperty(string json, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty(propertyName, out _);
            }
            catch
            {
                return false;
            }
        }
    }
}
