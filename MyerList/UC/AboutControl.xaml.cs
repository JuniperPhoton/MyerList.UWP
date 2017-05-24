using JP.Utils.Debug;
using JP.Utils.Helper;
using MyerList.Common;
using MyerList.Helper;
using MyerListUWP;
using MyerListUWP.Helper;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI.Xaml;

namespace MyerList.UC
{
    public sealed partial class AboutControl : NavigableUserControl
    {
        public AboutControl()
        {
            this.InitializeComponent();
            this.VersionTB.Text = App.AppVersion;
        }

        private async void FeedbackClick(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
                EmailMessage mes = new EmailMessage();
                mes.To.Add(rec);
                var attach = await Logger.GetLogFileAttachementAsync();
                if (attach != null)
                {
                    mes.Attachments.Add(attach);
                }
                var platform = DeviceHelper.IsDesktop ? "PC" : "Mobile";

                mes.Subject = $"MyerList for Windows 10 {platform}, {App.AppVersion} feedback, {DeviceHelper.OSVersion}, {DeviceHelper.DeviceModel}";
                await EmailManager.ShowComposeNewEmailAsync(mes);
            }
            catch (Exception ex)
            {
                var task = Logger.LogAsync(ex);
            }
        }

        private async void RateClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
        }
    }
}
