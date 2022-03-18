using System;
using System.Globalization;
using System.Windows.Data;

namespace PhotoPublisher
{
    internal class ValuesToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return Math.Round((double)value * 100,0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return double.Parse((string)value) / 100;
        }
    }
}
