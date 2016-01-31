using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyerList.Converter
{
    public class CateStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = value as SolidColorBrush;
            return new SolidColorBrush(new Color()
            {
                A = 100,
                R = (byte)(brush.Color.R * 0.4),
                G = (byte)(brush.Color.G * 0.4),
                B = (byte)(brush.Color.B * 0.4)
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
