using ChaoFunctionRT;
using MyerList.Base;
using MyerListUWP;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace MyerList
{
    
    public sealed partial class AboutPage : BindablePage
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        protected override void SetUpPageAnimation()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new ContinuumNavigationTransitionInfo();
            
            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpGrayTitleBar();
        }

        private async void FeedbackClick(object sender,RoutedEventArgs e)
        {
            try
            {
                EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
                EmailMessage mes = new EmailMessage();
                mes.To.Add(rec);
                mes.Subject = "MyerList for Windows 10 feedback";
                await EmailManager.ShowComposeNewEmailAsync(mes);
            }
            catch(Exception)
            {

            }
        }

        private async void RateClick(object sender, RoutedEventArgs e)
        {
            //await Launcher.LaunchUriAsync(
            //                 new Uri("ms-windows-store:reviewapp?appid=31eb52eb-aaee-43d9-b573-22ee91490502"));
            await Launcher.LaunchUriAsync(
                  new Uri("ms-windows-store://review/?ProductId=9nblggh11k1m"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TitleBarHelper.SetUpGrayTitleBar();
        }

    }
}
