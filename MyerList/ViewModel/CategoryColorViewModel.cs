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
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F75B44").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#EC4128").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F73215").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F7445B").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#E1184B").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#C11943").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#80224C").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#66436F").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#713A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#5F3A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#4D3A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#352F44").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#474E88").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2E3675").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2A2E51").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#417C98").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF6FD1FF").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF3CBBF7").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF217CDC").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF4CAFFF").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF5474C1").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#317CA0").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#39525F").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#4F9595").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2C8D8D").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF00BEBE").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#257575").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#2B8A78").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#3FBEA6").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#3FBE7D").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#1C9B5A").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#5A9849").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#739849").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#C9D639").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#D6CD00").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F7C142").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FFF7D842").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#F79E42").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FF8726").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.Hex2Color("#FFEF7919").Value));
        }
    }
}
