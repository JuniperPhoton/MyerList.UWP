using JP.Utils.Data;
using JP.Utils.Network;
using MyerList;
using MyerList.Helper;
using MyerList.Model;
using MyerListUWP.Helper;
using MyerListUWP.View;
using MyerListUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
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
        public static bool IsSyncListOnce { get; set; } = false;
        public static bool IsNoNetwork
        {
            get
            {
                return !NetworkHelper.HasNetWork;
            }
        }

        public static Frame ContentFrame = null;

        public static CategoryViewModel CurrentCateVM = null;

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
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

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                Window.Current.Content = rootFrame;

                ConfigHelper.CheckConfig();

                if (LocalSettingHelper.HasValue("AppLang") == false)
                {
                    var lang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
                    if (lang.Contains("zh"))
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                    }
                    else ApplicationLanguages.PrimaryLanguageOverride = "en-US";

                    LocalSettingHelper.AddValue("AppLang", ApplicationLanguages.PrimaryLanguageOverride);
                }
                else ApplicationLanguages.PrimaryLanguageOverride = LocalSettingHelper.GetValue("AppLang");


                if (LocalSettingHelper.HasValue("email"))
                {
                    rootFrame.Navigate(typeof(MainPage), LoginMode.Login);
                }
                else if (LocalSettingHelper.GetValue("OfflineMode") == "true")
                {
                    App.IsInOfflineMode = true;
                    rootFrame.Navigate(typeof(MainPage), LoginMode.OfflineMode);
                }
                else
                {
                    App.IsInOfflineMode = false;
                    rootFrame.Navigate(typeof(StartPage));
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
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

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
