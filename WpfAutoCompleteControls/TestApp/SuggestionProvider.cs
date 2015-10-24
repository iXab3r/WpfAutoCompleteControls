namespace TestApp
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Media;

    using WpfAutoCompleteControls.Editors;

    internal sealed class SuggestionProvider : ISuggestionProvider
    {
        private readonly ColorInfo[] knownColors;

        private readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

        public SuggestionProvider()
        {
            var colorsList = typeof (Colors)
                .GetProperties()
                .Where(x => x.PropertyType == typeof(Color))
                .Where(x => x.CanRead)
                .Select(x => (Color)x.GetValue(null))
                .ToArray();

            var colorProperties = typeof (Colors).GetProperties();

            knownColors = colorsList.Select(x => new ColorInfo()
            {
                Name = colorProperties.FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), x))?.Name ?? "Unknown",
                Color = x
            }).ToArray();
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return new object[0];
            }

            Thread.Sleep(Delay);

            filter = filter.Trim();
            return knownColors
                .Where(x => x.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }

    public class ColorInfo
    {
        public Color Color { get; set; }

        public string Name { get; set; }
    }
}