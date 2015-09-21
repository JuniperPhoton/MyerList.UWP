using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JP.Utils.Data;
using MyerList.Model;
using MyerListUWP;
using MyerListUWP.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerList.ViewModel
{
    public class StartViewModel:ViewModelBase
    {
        private RelayCommand _navigateToLoginCommand;
        public RelayCommand NavigateToLoginCommand
        {
            get
            {
                if (_navigateToLoginCommand != null)
                {
                    return _navigateToLoginCommand;
                }
                return _navigateToLoginCommand = new RelayCommand(() =>
                {
                    App.isInOfflineMode = false;
                    LocalSettingHelper.AddValue("OfflineMode", "false");

                    Frame rootFrame = Window.Current.Content as Frame;
                    if (rootFrame != null) rootFrame.Navigate(typeof(LoginPage),LoginMode.Login);
                });
            }
        }

        private RelayCommand _navigatToRegisterCommand;
        public RelayCommand NavigateToRegisterCommand
        {
            get
            {
                if (_navigatToRegisterCommand != null)
                {
                    return _navigatToRegisterCommand;
                }
                return _navigatToRegisterCommand = new RelayCommand(() =>
                {
                    App.isInOfflineMode = false;
                    LocalSettingHelper.AddValue("OfflineMode", "false");

                    Frame rootFrame = Window.Current.Content as Frame;
                    if (rootFrame != null) rootFrame.Navigate(typeof(LoginPage),LoginMode.Register);

                    
                });
            }
        }


        private RelayCommand _navigateToOfflinemodeCommand;
        public RelayCommand  NavigateToOfflineModeCommand
        {
            get
            {
                if (_navigateToOfflinemodeCommand != null)
                    return _navigateToOfflinemodeCommand;
                return _navigateToOfflinemodeCommand = new RelayCommand(() =>
                {
                    LocalSettingHelper.AddValue("OfflineMode", "true");
                    App.isInOfflineMode = true;

                    Frame rootFrame = Window.Current.Content as Frame;
                    Task.Delay(50);
                    rootFrame.Navigate(typeof(MainPage),LoginMode.OfflineMode);
                });
            }
        }

        public StartViewModel()
        {

        }
    }
}
