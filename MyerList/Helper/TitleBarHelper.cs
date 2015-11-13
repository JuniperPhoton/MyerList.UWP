using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace MyerListUWP.Helper
{
    public enum CateColors
    {
        DefaultColor,
        WorkColor,
        LifeColor,
        FamilyColor,
        EnterColor,
        DeletedColor
    }
    public static class TitleBarHelper
    {
        public static void SetUpTitleBar(Color foreColor)
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = foreColor;
            titleBar.ButtonHoverBackgroundColor = (new SolidColorBrush(new Color() {A=50,R=255,G=255,B=255 })).Color;
            titleBar.ButtonPressedBackgroundColor = (new SolidColorBrush(new Color() { A = 20, R = 255, G = 255, B = 255 })).Color;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = (new SolidColorBrush(new Color() { A = 50, R = 255, G = 255, B = 255 })).Color;
        }

        public static void SetUpBlackTitleBar()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Black;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = titleBar.BackgroundColor;
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = Colors.Black;
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = Colors.Black;
        }
    }
}
