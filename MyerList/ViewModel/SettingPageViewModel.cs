using GalaSoft.MvvmLight;
using Windows.UI.Xaml;
using Windows.Globalization;
using Windows.ApplicationModel.Resources.Core;
using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using MyerList.Interface;
using GalaSoft.MvvmLight.Command;
using MyerList.Helper;
using Windows.UI.Xaml.Controls;
using MyerListUWP;
using MyerListCustomControl;
using MyerListUWP.Common;
using Windows.Storage;
using System.Threading.Tasks;
using System;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace MyerList.ViewModel
{

    public class SettingPageViewModel : ViewModelBase, INavigable
    {

        private int _currentLanguage;
        public int CurrentLanguage
        {
            get
            {
                return _currentLanguage;
            }
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    ShowHint = Visibility.Visible;
                    ChangeLanguage();
                }
                RaisePropertyChanged(() => CurrentLanguage);
            }
        }

        private Visibility _showHint;
        public Visibility ShowHint
        {
            get
            {
                return _showHint;
            }
            set
            {
                if (_showHint != value)
                {
                    _showHint = value;
                }
                RaisePropertyChanged(() => ShowHint);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                if (_logoutCommand != null)
                {
                    return _logoutCommand;
                }

                return _logoutCommand = new RelayCommand(async () =>
                {
                    Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.CloseHam);

                    DialogService cdex = new DialogService(ResourcesHelper.GetResString("Notice"), ResourcesHelper.GetResString("LogoutContent"));
                    cdex.LeftButtonContent = ResourcesHelper.GetResString("Ok");
                    cdex.RightButtonContent = ResourcesHelper.GetResString("Cancel");
                    cdex.OnLeftBtnClick += (async(str) =>
                    {
                        App.IsSyncListOnce = false;

                        await ClearUserSettings();

                        App.MainVM.CurrentMainPage.NavigationCacheMode = NavigationCacheMode.Disabled;

                        cdex.Hide();
                        Frame rootFrame = Window.Current.Content as Frame;
                        rootFrame.BackStack.Clear();
                        if (rootFrame != null) rootFrame.Navigate(typeof(StartPage));
                    });
                    cdex.OnRightBtnClick += (() =>
                    {
                        cdex.Hide();
                    });
                    await cdex.ShowAsync();
                });
            }
        }

        private ObservableCollection<SolidColorBrush> _tileColors;
        public ObservableCollection<SolidColorBrush> TileColors
        {
            get
            {
                return _tileColors;
            }
            set
            {
                if (_tileColors != value)
                {
                    _tileColors = value;
                    RaisePropertyChanged(() => TileColors);
                }
            }
        }

        public SettingPageViewModel()
        {
            ShowHint = Visibility.Collapsed;

            var lang = LocalSettingHelper.GetValue("AppLang");
            if (lang.Contains("zh"))
            {
                CurrentLanguage = 1;
            }
            else CurrentLanguage = 0;
        }

        private void ChangeLanguage()
        {
            if (CurrentLanguage == 1)
            {
                ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                LocalSettingHelper.AddValue("AppLang", "zh-CN");
                var resourceContext = ResourceContext.GetForCurrentView();
                resourceContext.Reset();
            }
            else
            {
                ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                LocalSettingHelper.AddValue("AppLang", "en-US");
                var resourceContext = ResourceContext.GetForCurrentView();
                resourceContext.Reset();
            }
        }

        private async Task ClearUserSettings()
        {
            LocalSettingHelper.CleanUpAll();

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.TryGetFileAsync(SerializerFileNames.CategoryFileName);
            if (file != null)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        private void InitialTileColors()
        {
            TileColors = new ObservableCollection<SolidColorBrush>();

        }

        public void Activate(object param)
        {
        }

        public void Deactivate(object param)
        {
        }
    }
}
