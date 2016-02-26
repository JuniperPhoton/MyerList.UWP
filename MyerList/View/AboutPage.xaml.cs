using JP.Utils.Debug;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerListUWP.Helper;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
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
                var attach = await ExceptionHelper.GetLogFileAttachement();
                if(attach!= null)
                {
                    mes.Attachments.Add(attach);
                }
                var platform = DeviceHelper.IsDesktop ? "PC" : "Mobile";

                mes.Subject = $"MyerList for Windows 10 {platform}, {ResourcesHelper.GetDicString("AppVersion")} feedback, {DeviceHelper.OSVersion}, {DeviceHelper.DeviceModel}";
                await EmailManager.ShowComposeNewEmailAsync(mes);
            }
            catch (Exception ex)
            {
                var task = ExceptionHelper.WriteRecordAsync(ex, nameof(AboutPage), nameof(FeedbackClick));
            }
        }

        private async void RateClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (DeviceHelper.IsMobile)
            {
                StatusBarHelper.SetUpBlackStatusBar();
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
