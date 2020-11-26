using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Projet_IA_Voilier
{
    public class WindTypeBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Sortie("convert", value.ToString());
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Sortie("convertback", value.ToString());
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
                return Enum.Parse(targetType, targetValue);

            return null;
        }

        public void Sortie(string desc, object value)
        {
            string TAG = "WINDCONVERTER";
            Debug.WriteLine(new StringBuilder(TAG).Append(": ").Append(desc).Append(": ").Append(value.ToString()).ToString());
        }
    }
}
