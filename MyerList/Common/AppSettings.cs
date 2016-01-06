using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.Storage;

namespace MyerListUWP.Common
{
    public class AppSettings:ViewModelBase
    {
        public bool EnableTile
        {
            get
            {
                return ReadSettings(nameof(EnableTile), true);
            }
            set
            {
                SaveSettings(nameof(EnableTile), value);
                RaisePropertyChanged(() => EnableTile);
                if (value == true)
                {
                    Messenger.Default.Send(new GenericMessage<string>(""), "SyncExecute");
                }
                else
                {
                    HttpReqModule.UpdateTileHelper.ClearAllSchedules();
                }
            }
        }

        public bool EnableGesture
        {
            get
            {
                return ReadSettings(nameof(EnableGesture), true);
            }
            set
            {
                SaveSettings(nameof(EnableGesture), value);
                RaisePropertyChanged(() => EnableGesture);
            }
        }

        public bool ShowKeyboard
        {
            get
            {
                return ReadSettings(nameof(ShowKeyboard), true);
            }
            set
            {
                SaveSettings(nameof(ShowKeyboard), value);
                RaisePropertyChanged(() => ShowKeyboard);
            }
        }

        public bool IsAddToBottom
        {
            get
            {
                return ReadSettings(nameof(IsAddToBottom), true);
            }
            set
            {
                SaveSettings(nameof(IsAddToBottom), value);
                RaisePropertyChanged(() => IsAddToBottom);
            }
        }

        public bool EnableBackgroundTask
        {
            get
            {
                return ReadSettings(nameof(EnableBackgroundTask), true);
            }
            set
            {
                SaveSettings(nameof(EnableBackgroundTask), value);
                RaisePropertyChanged(() => EnableBackgroundTask);
            }
        }

        public ApplicationDataContainer LocalSettings { get; set; }

        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (defaultValue!=null)
            {
                return defaultValue;
            }
            return default(T);
        }

        private static readonly Lazy<AppSettings> lazy =new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance { get { return lazy.Value; } }
    }
}
