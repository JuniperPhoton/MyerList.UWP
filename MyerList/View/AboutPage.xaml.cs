using ChaoFunctionRT;
using JP.Utils.Debug;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerListUWP;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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

    public sealed partial class AboutPage : CustomTitleBarPage
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private async void FeedbackClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
                EmailMessage mes = new EmailMessage();
                mes.To.Add(rec);
                mes.Subject = "MyerList for Windows 10 feedback";
                await EmailManager.ShowComposeNewEmailAsync(mes);
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecordAsync(ex, nameof(AboutPage), nameof(FeedbackClick));
            }
        }

        private async void RateClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                  new Uri("ms-windows-store://review/?ProductId=9nblggh11k1m"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DeviceHelper.IsMobile)
            {
                StatusBarHelper.SetUpBlueStatusBar();
            }
            else
            {
                this.TitleBarUC.SetForegroundColor(Colors.Black);
                TitleBarHelper.SetUpForeBlackTitleBar();
            }
        }

        private async void stackPanel_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
                var msg = await ExceptionHelper.ReadRecordAsync();
                var task = ExceptionHelper.EraseRecord();
                EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
                EmailMessage mes = new EmailMessage();
                mes.To.Add(rec);
                mes.Subject = "MyerList for Windows 10 exception";
                mes.Body += msg;
                await EmailManager.ShowComposeNewEmailAsync(mes);
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecordAsync(ex, nameof(AboutPage), nameof(stackPanel_DoubleTapped));
            }
        }

        private void LogoImg_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
    }
}
