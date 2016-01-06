using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerList.ViewModel;
using MyerListUWP;
using MyerListUWP.Common;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerList
{
    public sealed partial class LoginPage : CustomTitleBarPage
    {
        public LoginViewModel LoginVM;

        public LoginPage()
        {
            this.InitializeComponent();
            this.KeyDown += LoginPage_KeyDown;

            LoginVM = new LoginViewModel();
            this.DataContext = LoginVM;

            if(ApiInformationHelper.HasStatusBar)
            {
                StatusBar.GetForCurrentView().BackgroundColor = (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush).Color;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0.01;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }
        }

        private void LoginPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.PressEnterToLoginToken);
                this.Focus(FocusState.Pointer);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }
    }
}
