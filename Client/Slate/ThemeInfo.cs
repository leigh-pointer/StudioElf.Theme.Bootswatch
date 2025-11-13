using Oqtane.Models;
using Oqtane.Themes;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.Slate
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Bootswatch Slate",
            Version = "6.5.3.8",
            ThemeSettingsType = "StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane",
            ContainerSettingsType = "StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane",
            PackageName = "StudioElf.Theme.Bootswatch",
            Resources = new List<Resource>()
            {
                new Stylesheet("https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.8/slate/bootstrap.min.css", "sha512-/3XXaUdpBwAtkHVWWFJFsJpKuexssgh25p8WVxMLY268QLt6P+dqZUT53NI+azlA2j35uSx2pDNL/0EoGkiM0Q==", "anonymous"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Theme.css"),
                new Stylesheet($"Themes/StudioElf.Theme.Bootswatch/Slate.css"),
                new Script("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/js/bootstrap.bundle.min.js", "sha512-HvOjJrdwNpDbkGJIG2ZNqDlVqMo77qbs4Me4cah0HoDrfhrbA+8SBlZn1KrvAQw7cILLPFJvdwIgphzQmMm+Pw==", "anonymous")
            }
        };
    }
}