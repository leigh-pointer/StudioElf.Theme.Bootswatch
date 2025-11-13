using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Globalization;
using Oqtane.Shared;

class Program
{
    // Configure this relative path to match your repo structure
    static string ClientProjectPath =
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Client")); // 
    static string BootstrapBundleFileName = "bootstrap.bundle.min.js";
    // -----------------------------------------------------------------------------
    // THEMES TO IGNORE (case-insensitive)
    // -----------------------------------------------------------------------------
    static readonly HashSet<string> IgnoredThemes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Cyborg"
        // Add more if needed: "Solar", "Vapor", etc.
    };

    static async Task<int> Main(string[] args)
    {
        try
        {
            if(args.Length > 0)
                ClientProjectPath = args[0];

            Console.WriteLine($"Client project path: {Path.GetFullPath(ClientProjectPath)}");
            string oqtaneVersion = Constants.Version; // e.g. "6.2.1"
            // Extract major number (6 from "6.2.1")
            var oqtaneMajorPart = oqtaneVersion.Split('.', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ??
                "6";
            int OqtaneMajorVersion = int.TryParse(oqtaneMajorPart, out var parsed) ? parsed : 6;

            using var http = new HttpClient();

            // 1) Fetch Bootswatch API
            Console.WriteLine("Fetching Bootswatch API...");
            var bootswatchJson = await http.GetStringAsync("https://bootswatch.com/api/5.json");
            using var bootsDoc = JsonDocument.Parse(bootswatchJson);

            var themesJson = bootsDoc.RootElement.GetProperty("themes").EnumerateArray().ToArray();
            var themeNames = themesJson
                .Select(t => t.GetProperty("name").GetString() ?? string.Empty)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .ToArray();

            Console.WriteLine($"Found {themeNames.Length} themes.");

            // 2) Fetch cdnjs info for Bootswatch
            Console.WriteLine("Fetching cdnjs assets for Bootswatch...");
            var cdnjsBootswatchJson = await http.GetStringAsync(
                "https://api.cdnjs.com/libraries/bootswatch?fields=version,assets");
            using var cdnBootsDoc = JsonDocument.Parse(cdnjsBootswatchJson);

            string cdnVersion = cdnBootsDoc.RootElement.GetProperty("version").GetString() ?? string.Empty;

            // Split into parts and build full combined version: 6.5.3.8
            var parts = cdnVersion.Split('.', StringSplitOptions.RemoveEmptyEntries);
            string combinedVersion = parts.Length switch
            {
                3 => $"{OqtaneMajorVersion}.{parts[0]}.{parts[1]}.{parts[2]}",
                2 => $"{OqtaneMajorVersion}.{parts[0]}.{parts[1]}.0",
                1 => $"{OqtaneMajorVersion}.{parts[0]}.0.0",
                _ => $"{OqtaneMajorVersion}.0.0.0"
            };
            Console.WriteLine($"Split into parts and build full combined version: {combinedVersion}");
            UpdateNuspecVersion(combinedVersion);

            Console.WriteLine($"cdnjs reports Bootswatch version: {cdnVersion}");

            var assets = cdnBootsDoc.RootElement.GetProperty("assets").EnumerateArray().ToArray();

            // Pick asset with matching version if possible
            JsonElement asset = default;
            var match = assets.FirstOrDefault(a => a.GetProperty("version").GetString() == cdnVersion);
            if(match.ValueKind != JsonValueKind.Undefined)
                asset = match;
            else if(assets.Length > 0)
                asset = assets.Last();
            else
                throw new Exception("No assets found in cdnjs Bootswatch response.");

            Dictionary<string, string> sriMap = new(StringComparer.OrdinalIgnoreCase);
            if(asset.TryGetProperty("sri", out var sriElem))
            {
                foreach(var prop in sriElem.EnumerateObject())
                    sriMap[prop.Name] = prop.Value.GetString() ?? string.Empty;
            }

            HashSet<string> fileList = new(StringComparer.OrdinalIgnoreCase);
            if(asset.TryGetProperty("files", out var filesElem))
            {
                foreach(var f in filesElem.EnumerateArray())
                    fileList.Add(f.GetString() ?? string.Empty);
            }

            // 3) Fetch Bootstrap info (for JS bundle integrity)
            Console.WriteLine("Fetching cdnjs info for Bootstrap...");
            string bootstrapVersion = string.Empty;
            string bootstrapSRI = string.Empty;

            try
            {
                var cdnjsBootstrapJson = await http.GetStringAsync(
                    "https://api.cdnjs.com/libraries/bootstrap?fields=version,assets");
                using var cdnBootDoc = JsonDocument.Parse(cdnjsBootstrapJson);

                bootstrapVersion = cdnBootDoc.RootElement.GetProperty("version").GetString() ?? string.Empty;
                var bAssets = cdnBootDoc.RootElement.GetProperty("assets").EnumerateArray().ToArray();

                JsonElement bAsset = default;
                var bMatch = bAssets.FirstOrDefault(a => a.GetProperty("version").GetString() == bootstrapVersion);
                if(bMatch.ValueKind != JsonValueKind.Undefined)
                    bAsset = bMatch;
                else if(bAssets.Length > 0)
                    bAsset = bAssets.Last();
                else
                    throw new Exception("No Bootstrap assets found.");

                if(bAsset.TryGetProperty("sri", out var bSriElem))
                {
                    foreach(var prop in bSriElem.EnumerateObject())
                    {
                        var name = prop.Name;
                        if(name.EndsWith("/" + BootstrapBundleFileName, StringComparison.OrdinalIgnoreCase) ||
                            name.Equals(BootstrapBundleFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            bootstrapSRI = prop.Value.GetString() ?? string.Empty;
                            break;
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine($"Warning: Bootstrap SRI fetch failed: {ex.Message}");
            }

            Console.WriteLine($"Bootstrap version (from cdnjs): {bootstrapVersion}");

            // 4) Generate or update ThemeInfo.cs for each Bootswatch theme
            foreach(var themeName in themeNames)
            {
                try
                {
                    if(IgnoredThemes.Contains(themeName))
                    {
                        Console.WriteLine($"Skipping ignored theme: {themeName}");
                        continue;
                    }

                    string slug = themeName.ToLowerInvariant().Replace(" ", "").Replace(".", "").Replace("/", "");
                    string title = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(themeName.ToLowerInvariant());
                    string themeFolder = Path.Combine(ClientProjectPath, title);

                    if(!Directory.Exists(themeFolder))
                    {
                        Console.WriteLine($"Creating folder: {themeFolder}");
                        Directory.CreateDirectory(themeFolder);
                    }

                    string outputFile = Path.Combine(themeFolder, "ThemeInfo.cs");
                    string cssUrl = $"https://cdnjs.cloudflare.com/ajax/libs/bootswatch/{cdnVersion}/{slug}/bootstrap.min.css";
                    string bootstrapBundleUrl = string.IsNullOrEmpty(bootstrapVersion)
                        ? $"https://cdnjs.cloudflare.com/ajax/libs/bootstrap/{cdnVersion}/js/{BootstrapBundleFileName}"
                        : $"https://cdnjs.cloudflare.com/ajax/libs/bootstrap/{bootstrapVersion}/js/{BootstrapBundleFileName}";

                    string cssKey = $"{slug}/bootstrap.min.css";
                    string cssSRI = sriMap.TryGetValue(cssKey, out var found) ? found : string.Empty;

                    if(string.IsNullOrEmpty(cssSRI))
                    {
                        Console.WriteLine($"Computing SRI for {cssUrl} ...");
                        cssSRI = await ComputeSha512IntegrityAsync(http, cssUrl);
                    }

                    string bsSRI = !string.IsNullOrEmpty(bootstrapSRI)
                        ? bootstrapSRI
                        : await ComputeSha512IntegrityAsync(http, bootstrapBundleUrl);

                    string content = GenerateThemeInfoCode(title, combinedVersion, cssUrl, cssSRI, bootstrapBundleUrl, bsSRI);
                    File.WriteAllText(outputFile, content, Encoding.UTF8);
                    Console.WriteLine($"Wrote {outputFile}");

                    // Ensure theme has Containers/ and Themes/ structure
                    EnsureThemeStructure(themeFolder, title);

                    // Ensure CSS in wwwroot
                    await EnsureThemeCssAsync(http, title, cdnVersion);

                } catch(Exception ex)
                {
                    Console.WriteLine($"Error processing theme '{themeName}': {ex.Message}");
                }
            }

            Console.WriteLine("✅ Bootswatch themes updated successfully.");
            return 0;
        } catch(Exception ex)
        {
            Console.WriteLine("❌ Fatal error: " + ex);
            return 2;
        }
    }

    static void UpdateNuspecVersion(string combinedVersion)
    {
        // Adjust the relative path if needed to point at your .nuspec file
        string nuspecPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "Package",
                "Oqtane.Theme.Bootswatch.nuspec"));

        if(!File.Exists(nuspecPath))
        {
            Console.WriteLine($"⚠️  Could not find nuspec file at {nuspecPath}");
            return;
        }

        string content = File.ReadAllText(nuspecPath);

        // Replace existing <version>...</version> tag
        string updated = System.Text.RegularExpressions.Regex
            .Replace(
                content,
                @"<version>.*?</version>",
                $"<version>{combinedVersion}</version>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if(updated != content)
        {
            File.WriteAllText(nuspecPath, updated, Encoding.UTF8);
            Console.WriteLine($"✅ Updated nuspec version to {combinedVersion}");
        } else
        {
            Console.WriteLine("⚠️  nuspec version unchanged or tag not found.");
        }
    }

    static async Task<string> ComputeSha512IntegrityAsync(HttpClient http, string url)
    {
        try
        {
            var bytes = await http.GetByteArrayAsync(url);
            using var sha = SHA512.Create();
            var hash = sha.ComputeHash(bytes);
            var base64 = Convert.ToBase64String(hash);
            return "sha512-" + base64;
        } catch(Exception ex)
        {
            Console.WriteLine($"Failed to compute integrity for {url}: {ex.Message}");
            return string.Empty;
        }
    }

    static string GenerateThemeInfoCode(
        string themeTitle,
        string version,
        string cssUrl,
        string cssIntegrity,
        string bundleUrl,
        string bundleIntegrity)
    {
        return $@"using Oqtane.Models;
using Oqtane.Themes;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.{themeTitle}
{{
    public class ThemeInfo : ITheme
    {{
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {{
            Name = ""Bootswatch {themeTitle}"",
            Version = ""{version}"",
            ThemeSettingsType = ""StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane"",
            ContainerSettingsType = ""StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane"",
            PackageName = ""StudioElf.Theme.Bootswatch"",
            Resources = new List<Resource>()
            {{
                new Stylesheet(""{cssUrl}"", ""{cssIntegrity}"", ""anonymous""),
                new Stylesheet(""Themes/StudioElf.Theme.Bootswatch/Theme.css""),
                new Stylesheet($""Themes/StudioElf.Theme.Bootswatch/{themeTitle}.css""),
                new Script(""{bundleUrl}"", ""{bundleIntegrity}"", ""anonymous"")
            }}
        }};
    }}
}}";
    }

    // -----------------------------------------------------------------------------
    // TEMPLATE HELPERS: Create default .razor files for new themes
    // -----------------------------------------------------------------------------
    static void EnsureThemeStructure(string themeFolder, string themeName)
    {
        // Containers folder
        string containersPath = Path.Combine(themeFolder, "Containers");
        if(!Directory.Exists(containersPath))
            Directory.CreateDirectory(containersPath);

        string containerFile = Path.Combine(containersPath, "Container.razor");
        if(!File.Exists(containerFile))
        {
            string containerContent =
    $@"@namespace StudioElf.Theme.Bootswatch.{themeName}
@inherits StudioElf.Theme.Bootswatch.Default.Container

@{{
    base.BuildRenderTree(__builder);
}}";
            File.WriteAllText(containerFile, containerContent, Encoding.UTF8);
            Console.WriteLine($"🧱 Created: {containerFile}");
        }

        // Themes folder
        string themesPath = Path.Combine(themeFolder, "Themes");
        if(!Directory.Exists(themesPath))
            Directory.CreateDirectory(themesPath);

        string defaultFile = Path.Combine(themesPath, "Default.razor");
        if(!File.Exists(defaultFile))
        {
            string defaultContent =
    $@"@namespace StudioElf.Theme.Bootswatch.{themeName}
@inherits StudioElf.Theme.Bootswatch.Default.Default

@{{
    base.BuildRenderTree(__builder);
}}";
            File.WriteAllText(defaultFile, defaultContent, Encoding.UTF8);
            Console.WriteLine($"🧱 Created: {defaultFile}");
        }
    }

    // -----------------------------------------------------------------------------
    // ENSURE THEME CSS IN wwwroot (copy default.css if missing)
    // -----------------------------------------------------------------------------
    static async Task EnsureThemeCssAsync(HttpClient http, string themeName, string cdnVersion)
    {
        // Locate the Bootswatch CSS folder
        string wwwrootThemesFolder = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "..",
            "Client", "wwwroot", "Themes", "StudioElf.Theme.Bootswatch"));

        if (!Directory.Exists(wwwrootThemesFolder))
        {
            Directory.CreateDirectory(wwwrootThemesFolder);
            Console.WriteLine($"📁 Created missing folder: {wwwrootThemesFolder}");
        }

        string targetCssPath = Path.Combine(wwwrootThemesFolder, $"{themeName}.css");
        string defaultCssPath = Path.Combine(wwwrootThemesFolder, "default.css");

        // If target CSS already exists — nothing to do
        if (File.Exists(targetCssPath))
        {
            Console.WriteLine($"ℹ️  CSS already exists for {themeName}: {targetCssPath}");
            return;
        }

        // Copy default.css → ThemeName.css
        if (File.Exists(defaultCssPath))
        {
            try
            {
                File.Copy(defaultCssPath, targetCssPath);
                Console.WriteLine($"✅ Copied default.css → {Path.GetFileName(targetCssPath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️  Failed to copy default.css for {themeName}: {ex.Message}");
            }
        }
        else
        {
            // Fallback — default.css missing
            try
            {
                await File.WriteAllTextAsync(
                    targetCssPath,
                    $"/* Missing default.css, created placeholder for {themeName} */{Environment.NewLine}",
                    Encoding.UTF8);
                Console.WriteLine($"⚠️  default.css not found — created empty placeholder: {targetCssPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to create placeholder CSS for {themeName}: {ex.Message}");
            }
        }
    }

}
