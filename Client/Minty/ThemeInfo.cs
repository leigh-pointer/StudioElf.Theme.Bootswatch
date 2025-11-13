using Oqtane.Models;
using Oqtane.Themes;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.Minty
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Bootswatch Minty",
            Version = "6.5.3.8",
            ThemeSettingsType = "StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane",
            ContainerSettingsType = "StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane",
            PackageName = "StudioElf.Theme.Bootswatch",
            Resources = new List<Resource>()
            {
                new Stylesheet("https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.8/minty/bootstrap.min.css", "sha512-2rlhob8L606Mt1c8oB20JR78cIX27wzWelusr5yq+4VX6ly4F3Tr88Vhj8B63yxB7k8i1BRq3fKaiaYESpo8rA==", "anonymous"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Theme.css"),
                new Stylesheet($"Themes/StudioElf.Theme.Bootswatch/Minty.css"),
                new Script("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/js/bootstrap.bundle.min.js", "sha512-HvOjJrdwNpDbkGJIG2ZNqDlVqMo77qbs4Me4cah0HoDrfhrbA+8SBlZn1KrvAQw7cILLPFJvdwIgphzQmMm+Pw==", "anonymous")
            }
        };
    }
}