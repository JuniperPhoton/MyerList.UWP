using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerList.ViewModel;
using MyerListUWP.Helper;
using Windows.UI;
using Windows.UI.Xaml.Navigation;

namespace MyerList
{
    public sealed partial class SettingPage : CustomTitleBarPage
    {
        private SettingPageViewModel SettingVM
        {
            get
            {
                return this.DataContext as SettingPageViewModel;
            }
        }

        public SettingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(DeviceHelper.IsMobile)
            {
               StatusBarHelper.SetUpBlackStatusBar();
            }
            else
            {
                this.TitleBarUC.SetForegroundColor(Colors.Black);
                TitleBarHelper.SetUpTitleBarColorForDarkText();
            }
        }
    }
}
