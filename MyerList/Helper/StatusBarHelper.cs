using MyerListUWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace MyerList.Helper
{
    public class StatusBarHelper
    {
        public static StatusBar sb = StatusBar.GetForCurrentView();
        
        public static void SetUpBlueStatusBar()
        {
            sb.BackgroundOpacity = 1.0;
            sb.BackgroundColor = (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush).Color;
            sb.ForegroundColor = Colors.White;
        }

        public static void SetUpStatusBar(string color)
        {
            sb.BackgroundOpacity = 0;
            sb.ForegroundColor = Colors.White;
            //sb.BackgroundOpacity = 1.0;
            //sb.BackgroundColor = (App.Current.Resources[color] as SolidColorBrush).Color;
            //sb.ForegroundColor = Colors.White;
        }
    }
}
