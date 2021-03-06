﻿namespace WpfAutoCompleteControls.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal sealed class ExceptionToMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var exception = value as Exception;
            return exception?.Message;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}