using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.ViewModel;
using MyerListUWP;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using System;
using System.Numerics;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MyerList
{
    public sealed partial class LoginPage : CustomTitleBarPage
    {
        private LoginViewModel LoginVM;

        private Compositor _compositor;
        private Visual _mainVisual;

        public LoginPage()
        {
            this.InitializeComponent();
            this.KeyDown += LoginPage_KeyDown;

            this.DataContext = LoginVM = new LoginViewModel();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _mainVisual = ElementCompositionPreview.GetElementVisual(MainBorder);
            _mainVisual.Offset = new Vector3(200f, 0f, 0f);
            _mainVisual.Opacity = 0;

            if (ApiInformationHelper.HasStatusBar)
            {
                StatusBar.GetForCurrentView().BackgroundColor = (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush).Color;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0.01;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }

            this.Loaded += LoginPage_Loaded;
        }

        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 0f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(200);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, 1f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(200);

            _mainVisual.StartAnimation("Opacity", fadeAnimation);
            _mainVisual.StartAnimation("Offset.X", offsetAnimation);
        }

        private void LoginPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.PressEnterToLoginToken);
                this.Focus(FocusState.Pointer);
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
