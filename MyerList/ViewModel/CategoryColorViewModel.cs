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
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F75B44").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#EC4128").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F73215").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F7445B").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#E1184B").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#C11943").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#80224C").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#66436F").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#713A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#5F3A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#4D3A80").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#352F44").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#474E88").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2E3675").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2A2E51").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#417C98").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF6FD1FF").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF3CBBF7").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF217CDC").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF4CAFFF").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF5474C1").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#317CA0").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#39525F").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#4F9595").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2C8D8D").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF00BEBE").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#257575").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#2B8A78").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#3FBEA6").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#3FBE7D").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#1C9B5A").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#5A9849").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#739849").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#C9D639").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#D6CD00").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F7C142").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FFF7D842").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#F79E42").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FF8726").Value));
            CateColors.Add(new SolidColorBrush(ColorConverter.HexToColor("#FFEF7919").Value));
        }
    }
}
