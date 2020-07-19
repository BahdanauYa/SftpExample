using System;
using System.Windows;
using System.Windows.Data;

namespace SftpExample.Tools
{
    [ValueConversion(typeof(long), typeof(string))]
    public class PrettyBytesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is long size)
            {
                return LongToString(size);
            }

            return true;
        }

        private string LongToString(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            var result = $"{size:0.##} {sizes[order]}";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
