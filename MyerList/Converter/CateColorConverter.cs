using MyerListUWP;
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
    public class CateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((int)value)
            {
                case 0:
                    {
                        return (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush);
                    };
                case 1:
                    {
                        return (App.Current.Resources["WorkColor"] as SolidColorBrush);
                    };
                case 2:
                    {
                        return (App.Current.Resources["LifeColor"] as SolidColorBrush);
                    };
                case 3:
                    {
                        return (App.Current.Resources["FamilyColor"] as SolidColorBrush);
                    }; 
                case 4:
                    {
                        return (App.Current.Resources["EnterColor"] as SolidColorBrush);
                    }; 
                
            }
            return (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
