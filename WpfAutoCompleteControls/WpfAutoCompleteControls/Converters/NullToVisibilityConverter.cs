namespace WpfAutoCompleteControls.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    internal sealed class NullToVisibilityConverter : IValueConverter
    {
        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        private Visibility InvertedNullValue => NullValue == Visibility.Collapsed
            ? Visibility.Visible
            : Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNull = value is string ? string.IsNullOrWhiteSpace(value as string) : value == null;

            return isNull ? NullValue : InvertedNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}