using Oqtane.Models;
using Oqtane.Themes;
using System.Collections.Generic;

namespace StudioElf.Theme.Bootswatch.Morph
{
    public class ThemeInfo : ITheme
    {
        public Oqtane.Models.Theme Theme => new Oqtane.Models.Theme
        {
            Name = "Bootswatch Morph",
            Version = "6.5.3.8",
            ThemeSettingsType = "StudioElf.Theme.Bootswatch.ThemeSettings, StudioElf.Theme.Bootswatch.Oqtane",
            ContainerSettingsType = "StudioElf.Theme.Bootswatch.ContainerSettings, StudioElf.Theme.Bootswatch.Oqtane",
            PackageName = "StudioElf.Theme.Bootswatch",
            Resources = new List<Resource>()
            {
                new Stylesheet("https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.8/morph/bootstrap.min.css", "sha512-WNJfWY22WFbNJiO14ae1aIQUwHjbkYaXw5umKIwi8YzyGrwjIuv262hC/fnOKO4qlEU0ojnn+f4sFL+b12Gmbw==", "anonymous"),
                new Stylesheet("Themes/StudioElf.Theme.Bootswatch/Theme.css"),
                new Stylesheet($"Themes/StudioElf.Theme.Bootswatch/Morph.css"),
                new Script("https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.8/js/bootstrap.bundle.min.js", "sha512-HvOjJrdwNpDbkGJIG2ZNqDlVqMo77qbs4Me4cah0HoDrfhrbA+8SBlZn1KrvAQw7cILLPFJvdwIgphzQmMm+Pw==", "anonymous")
            }
        };
    }
}