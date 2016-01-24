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

        public static void SetUpStatusBar()
        {
            sb.BackgroundOpacity = 0;
            sb.ForegroundColor = Colors.White;
        }
    }
}
