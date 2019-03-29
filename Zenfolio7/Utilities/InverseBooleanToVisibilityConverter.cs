using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zenfolio7.Utilities
{
    public class InverseBooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static InverseBooleanToVisibilityConverter converter;
        public InverseBooleanToVisibilityConverter()
        {
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                Visibility visibility = (Visibility)value;
                if (visibility == Visibility.Collapsed)
                {
                    return true;
                }
            }
            return false;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new InverseBooleanToVisibilityConverter());
        }
    }
}
