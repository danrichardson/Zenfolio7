using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Zenfolio7.Utilities
{
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static BooleanToVisibilityConverter converter;
        public BooleanToVisibilityConverter()
        {
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                Visibility visibility = (Visibility)value;
                if (visibility == Visibility.Collapsed)
                {
                    return false;
                }
            }
            return true;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return converter ?? (converter = new BooleanToVisibilityConverter());
        }
    }
}
