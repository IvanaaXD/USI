using LangLang.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LangLang.View.Converters
{
    public class LevelOfLanguageToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<LanguageLevel> levelsOfLanguagelanguages)
            {
                var levelsOfLanguagelanguagesString = levelsOfLanguagelanguages.Select(day => day.ToString());

                return string.Join(", ", levelsOfLanguagelanguagesString);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
