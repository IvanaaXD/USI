using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace LangLang.View.Converters
{
    public class PasswordValidationToStringConverter : IValueConverter
    {
        private Regex _passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{5,}$");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string password && _passwordRegex.IsMatch(password))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
