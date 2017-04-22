using JP.Utils.UI;
using Windows.UI;
using Windows.UI.ViewManagement;

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
    }
}
