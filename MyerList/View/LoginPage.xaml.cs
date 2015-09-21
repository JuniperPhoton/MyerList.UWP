using GalaSoft.MvvmLight.Messaging;
using MyerList.Base;
using MyerList.Helper;
using MyerList.ViewModel;
using MyerListUWP;
using MyerListUWP.Helper;
using Windows.UI;
using Windows.UI.ViewManagement;
#if WINDOWS_PHONE_APP
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.ViewManagement;
#endif
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MyerList
{

    public sealed partial class LoginPage : BindablePage
    {
        public LoginViewModel LoginVM;
        public LoginPage()
        {
            this.InitializeComponent();
            this.KeyDown += LoginPage_KeyDown;

            LoginVM = new LoginViewModel();
            this.DataContext = LoginVM;

            if(ApiInformationHelper.HasStatusBar())
            {
                StatusBar.GetForCurrentView().BackgroundColor = (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush).Color;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0.01;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }

            Messenger.Default.Register<GenericMessage<string>>(this, "toast", act =>
            {
                var msg = act.Content;
                ToastControl.ShowMessage(msg);
            });
        }

        private void LoginPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.PressEnterToLoginToken);
            }
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpGrayTitleBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }


    }
}
