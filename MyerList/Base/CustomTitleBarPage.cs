using JP.Utils.Helper;
using MyerList.UC;
using MyerListUWP.Helper;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyerList.Base
{
    public class CustomTitleBarPage : BindablePage
    {
        protected TitleBarControl TitleBarUc;

        public CustomTitleBarPage()
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DeviceHelper.IsDesktop)
            {
                TitleBarUc = TitleBarHelper.CustomTitleBar(this.Content);
                TitleBarUc.OnClickBackBtn += (s, ex) =>
                {
                    if (Frame.CanGoBack) Frame.GoBack();
                };
            }
        }
    }
}
