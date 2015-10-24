namespace TestApp
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Threading;
    using System.Windows.Media;

    using WpfAutoCompleteControls.Editors;

    internal sealed class SuggestionProvider : ISuggestionProvider
    {
        private readonly ColorInfo[] knownColors;

        private readonly TimeSpan Delay = TimeSpan.FromSeconds(5);

        public SuggestionProvider()
        {
            var colorsList = typeof (System.Windows.SystemColors)
                .GetProperties()
                .Where(x => x.PropertyType == typeof(Color))
                .Where(x => x.CanRead)
                .Select(x => (Color)x.GetValue(null))
                .ToArray();

            knownColors = colorsList.Select(x => new ColorInfo() {Name = x.ToString(), Color = x}).ToArray();
        }

        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }

            Thread.Sleep(Delay);

            return knownColors.Take(10);
        }
    }

    public class ColorInfo
    {
        public Color Color { get; set; }

        public string Name { get; set; }
    }
}