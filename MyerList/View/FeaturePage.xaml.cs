using JP.Utils.Data;
using MyerList;
using MyerList.Base;
using MyerList.Model;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyerListUWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeaturePage : BindablePage
    {
        public FeaturePage()
        {
            this.InitializeComponent();
        }


        protected override void SetNavigationBackBtn()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            LocalSettingHelper.AddValue(App.Current.Resources["FeatureToken"] as string, "1");
            if (LocalSettingHelper.HasValue("email"))
            {
                Frame.Navigate(typeof(MainPage), LoginMode.Login);
            }
            else if (LocalSettingHelper.GetValue("OfflineMode") == "true")
            {
                App.IsInOfflineMode = true;
                Frame.Navigate(typeof(MainPage), LoginMode.OfflineMode);
            }
            else
            {
                App.IsInOfflineMode = false;
                Frame.Navigate(typeof(StartPage));
            }
        }
    }
}
