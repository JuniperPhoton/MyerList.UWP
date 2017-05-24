using JP.Utils.UI;
using MyerList.UC;
using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        public static void SetUpTitleBarColorForDarkText()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Black;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#f5f5f5").Value;
            titleBar.ButtonHoverForegroundColor= Colors.Black;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#BBBBBB").Value;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = ColorConverter.HexToColor("#676767").Value;
        }

        public static void SetUpTitleBarColorForLightText()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.White;
            titleBar.ButtonHoverBackgroundColor = ColorConverter.HexToColor("#31DEDEDE");
            titleBar.ButtonHoverForegroundColor = Colors.White;
            titleBar.ButtonPressedBackgroundColor = ColorConverter.HexToColor("#22DEDEDE");
        }

        public static TitleBarControl CustomTitleBar(UIElement rootContent)
        {
            var currentContent = rootContent as Grid;
            if (currentContent == null)
            {
                throw new ArgumentNullException("The root element of the page should be Grid.");
            }
            var uc = new TitleBarControl();
            (currentContent as Grid).Children.Add(uc);
            Grid.SetColumnSpan(uc, 5);
            Grid.SetRowSpan(uc, 5);

            return uc;
        }
    }
}
