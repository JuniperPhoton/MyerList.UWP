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
using JP.Utils.UI;
using MyerListShared;

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
                    cdex.OnLeftBtnClick += (async (str) =>
                    {
                        App.HasSyncedListOnce = false;

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

            InitialTileColors();
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
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F75B44").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#EC4128").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F73215").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F7445B").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#E1184B").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#C11943").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#80224C").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#66436F").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#713A80").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#5F3A80").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#4D3A80").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#352F44").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#474E88").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2E3675").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2A2E51").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#417C98").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF6FD1FF").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF3CBBF7").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF217CDC").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF4CAFFF").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF5474C1").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#317CA0").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#39525F").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#4F9595").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2C8D8D").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF00BEBE").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#257575").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2B8A78").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#3FBEA6").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#3FBE7D").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#1C9B5A").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#5A9849").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#739849").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#C9D639").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#D6CD00").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F7C142").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FFF7D842").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F79E42").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF8726").Value));
            TileColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FFEF7919").Value));
        }

        public void Activate(object param)
        {
        }

        public void Deactivate(object param)
        {
        }

        public void Loaded(object param)
        {

        }
    }
}
