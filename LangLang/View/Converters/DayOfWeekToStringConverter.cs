using System;
using System.Globalization;
using System.Windows.Data;

namespace LangLang.View.Converters
{
    public class DayOfWeekToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DayOfWeek)
            {
                DayOfWeek dayOfWeek = (DayOfWeek)value;
                return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
