using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Media;
using MyerList.Model;
using MyerList.Helper;
using JP.Utils.Data;
using JP.Utils.Debug;
using MyerList.Interface;
using MyerListUWP;
using MyerListUWP.ViewModel;
using MyerListCustomControl;
using System.Runtime.InteropServices;
using JP.Utils.Helper;
using MyerListUWP.Common;
using JP.UWP.CustomControl;
using MyerList.UC;
using JP.Utils.UI;

namespace MyerList.ViewModel
{
    public class CategoryColorViewModel:ViewModelBase
    {
        private ObservableCollection<SolidColorBrush> _cateColors;
        public ObservableCollection<SolidColorBrush> CateColors
        {
            get
            {
                return _cateColors;
            }
            set
            {
                if (_cateColors != value)
                {
                    _cateColors = value;
                    RaisePropertyChanged(() => CateColors);
                }
            }
        }

        public CategoryColorViewModel()
        {
            CateColors = new ObservableCollection<SolidColorBrush>();
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F75B44")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#EC4128")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F73215")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F7445B")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#E1184B")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#C11943")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#80224C")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#66436F")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#713A80")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#5F3A80")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#4D3A80")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#352F44")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#474E88")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2E3675")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2A2E51")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#417C98")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF6FD1FF")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF3CBBF7")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF217CDC")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF4CAFFF")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF5474C1")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#317CA0")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#39525F")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#4F9595")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2C8D8D")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF00BEBE")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#257575")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2B8A78")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#3FBEA6")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#3FBE7D")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#1C9B5A")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#5A9849")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#739849")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#C9D639")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#D6CD00")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F7C142")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FFF7D842")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F79E42")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF8726")));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FFEF7919")));
        }
    }
}
