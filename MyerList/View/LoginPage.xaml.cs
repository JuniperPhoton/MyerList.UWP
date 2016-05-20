using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.ViewModel;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using System;
using Windows.System;
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
        private LoginViewModel LoginVM;

        public LoginPage()
        {
            this.InitializeComponent();
            this.KeyDown += LoginPage_KeyDown;

            this.DataContext = LoginVM = new LoginViewModel();
        }

        private void LoginPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.PressEnterToLoginToken);
                this.Focus(FocusState.Pointer);
            }
        }

        protected override void SetUpStatusBar()
        {
            if (APIInfoHelper.HasStatusBar)
            {
                StatusBar.GetForCurrentView().BackgroundColor = Colors.Transparent;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0.01;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.Black;
            }
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpForeWhiteTitleBar();
        }

        protected override void SetUpNavigationCache()
        {
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }

        private async void PrivacyBtn_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://juniperphoton.net/myerlist/privacy.html"));
        }
    }
}
