using Oqtane.Models;
using Oqtane.Themes;
using StudioElf.Theme.Bootswatch.Client;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.Default
{
    public class ThemeInfo : ITheme   
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Bootswatch Default",
            Version = VersionInfo.Version,
            ThemeSettingsType = "StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane",
            ContainerSettingsType = "StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane",
            PackageName = "StudioElf.Theme.Bootswatch",
            Resources = new List<Resource>()
            {
		        // obtained from https://cdnjs.com/libraries
                new Stylesheet("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/css/bootstrap.min.css",
                    "sha512-2bBQCjcnw658Lho4nlXJcc6WkV/UxpE/sAokbXPxQNGqmNdQrWqtw26Ns9kFF/yG792pKR1Sx8/Y1Lf1XN4GKA==", "anonymous"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Theme.css"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Default.css"),
                new Script("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/js/bootstrap.bundle.min.js",
                    "sha512-HvOjJrdwNpDbkGJIG2ZNqDlVqMo77qbs4Me4cah0HoDrfhrbA+8SBlZn1KrvAQw7cILLPFJvdwIgphzQmMm+Pw==", "anonymous")
            }
        };
    }
}