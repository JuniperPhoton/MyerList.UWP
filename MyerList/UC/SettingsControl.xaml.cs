using JP.Utils.Helper;
using MyerList.Common;
using MyerList.Helper;
using MyerList.ViewModel;
using MyerListUWP.Helper;
using Windows.UI.Xaml;

namespace MyerList.UC
{
    public sealed partial class SettingsControl : NavigableUserControl
    {
        private SettingPageViewModel SettingVM
        {
            get
            {
                return (Application.Current.Resources["Locator"] as ViewModelLocator).SettingVM;
            }
        }

        public SettingsControl()
        {
            this.InitializeComponent();
        }
    }
}
