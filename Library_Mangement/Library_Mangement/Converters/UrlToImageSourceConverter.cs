using FFImageLoading.Svg.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Converters
{
    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is string)
            {
                var filePathString = (string)value;
                if (filePathString.ToLowerInvariant().EndsWith(".svg"))
                {
                    return SvgImageSource.FromFile(filePathString, 50, 50);
                }
                else
                {
                    return ImageSource.FromFile(filePathString);
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
