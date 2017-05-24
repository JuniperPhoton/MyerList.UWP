using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using System;
using Windows.Storage;

namespace MyerListUWP.Common
{
    public class AppSettings : ViewModelBase
    {
        public static string DefaultCateJsonString = "{ \"modified\":true, \"cates\":[{\"name\":\"Work\",\"color\":\"#FF436998\",\"id\":1},{\"name\":\"Life\",\"color\":\"#FFFFB542\",\"id\":2},{\"name\":\"Family\",\"color\":\"#FFFF395F\",\"id\":3},{\"name\":\"Entertainment\",\"color\":\"#FF55C1C1\",\"id\":4}]}";
        public static string ModifiedCateJsonStringFore = "{ \"modified\":true, \"cates\":";

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
            if (defaultValue != null)
            {
                return defaultValue;
            }
            return default(T);
        }

        private static readonly Lazy<AppSettings> lazy = new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance { get { return lazy.Value; } }
    }
}
