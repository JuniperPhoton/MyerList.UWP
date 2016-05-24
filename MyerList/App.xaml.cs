using JP.Utils.Data;
using JP.Utils.Network;
using MyerList;
using MyerList.Common;
using MyerList.Helper;
using MyerList.Model;
using MyerList.ViewModel;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using MyerListUWP.View;
using MyerListUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using UmengSDK;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.System.UserProfile;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerListUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 
        public static bool IsInOfflineMode { get; set; } = false;

        public static bool HasSyncedListOnce { get; set; } = false;

        public static MainViewModel MainVM { get; set; }

        public static bool IsNoNetwork
        {
            get
            {
                return !NetworkHelper.HasNetWork;
            }
        }

        public static bool CanSendRequest
        {
            get
            {
                return (!IsNoNetwork || !IsInOfflineMode);
            }
        }

        public static Frame ContentFrame = null;

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            UmengAnalytics.TrackException(e.Exception);
        }

        private void App_Resuming(object sender, object e)
        {
            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            var appView = ApplicationView.GetForCurrentView();
            appView.SetPreferredMinSize(new Size(400, 700));

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;

                GlobalHelper.SetupLang();

                if (LocalSettingHelper.HasValue("email"))
                {
                    rootFrame.Navigate(typeof(MainPage), LoginMode.Login);
                }
                else if (LocalSettingHelper.GetValue("OfflineMode") == "true")
                {
                    IsInOfflineMode = true;
                    rootFrame.Navigate(typeof(MainPage), LoginMode.OfflineMode);
                }
                else
                {
                    IsInOfflineMode = false;
                    rootFrame.Navigate(typeof(StartPage));
                }
            }
            Window.Current.Activate();

            await UmengAnalytics.StartTrackAsync(UmengKey.UMENG_APP_KEY, "Marketplace");
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            await UmengAnalytics.StartTrackAsync(UmengKey.UMENG_APP_KEY, "Marketplace");
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            await UmengAnalytics.EndTrackAsync();
            deferral.Complete();
        }
    }
}
