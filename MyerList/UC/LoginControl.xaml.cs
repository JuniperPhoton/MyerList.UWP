using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerList.UC
{
    public sealed partial class LoginControl : UserControl
    {
        public event Action OnClickBackBtn;

        public LoginControl()
        {
            this.InitializeComponent();
        }

        private async void PrivacyBtn_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://juniperphoton.net/myerlist/privacy.html"));
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickBackBtn?.Invoke();
        }
    }
}
