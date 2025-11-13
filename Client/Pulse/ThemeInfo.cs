using Oqtane.Models;
using Oqtane.Themes;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.Pulse
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Bootswatch Pulse",
            Version = "6.5.3.8",
            ThemeSettingsType = "StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane",
            ContainerSettingsType = "StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane",
            PackageName = "StudioElf.Theme.Bootswatch",
            Resources = new List<Resource>()
            {
                new Stylesheet("https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.8/pulse/bootstrap.min.css", "sha512-gy/x03AxhuVCrGyK65V+7BZ4XCdxzzvw3Zal/IqLdJROJPnd4gTnwzkF1IJbq+RKXKLssk8N8QShZ0kvwnC6UA==", "anonymous"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Theme.css"),
                new Stylesheet($"Themes/StudioElf.Theme.Bootswatch/Pulse.css"),
                new Script("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/js/bootstrap.bundle.min.js", "sha512-HvOjJrdwNpDbkGJIG2ZNqDlVqMo77qbs4Me4cah0HoDrfhrbA+8SBlZn1KrvAQw7cILLPFJvdwIgphzQmMm+Pw==", "anonymous")
            }
        };
    }
}