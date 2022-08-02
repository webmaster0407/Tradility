using System;
using System.Globalization;
using System.Windows.Data;

namespace Tradility.UI.Converters
{
    public class DateTimeOffsetToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTimeOffset)value).DateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new DateTimeOffset((DateTime)value);
        }
    }
}
