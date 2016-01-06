using GalaSoft.MvvmLight;
using MyerList.Helper;
using Windows.UI.Xaml.Media;

namespace MyerListUWP.Model
{
    public class ToDoCategory:ViewModelBase
    {
        private string _cateName;
        public string CateName
        {
            get
            {
                if(_cateName=="Default")
                {
                    return ResourcesHelper.GetResString("DefaultCateName");
                }
                else return _cateName;
            }
            set
            {
                if (_cateName != value)
                {
                    _cateName = value;
                    RaisePropertyChanged(() => CateName);
                }
            }
        }

        private int _cateColorID;
        public int CateColorID
        {
            get
            {
                return _cateColorID;
            }
            set
            {
                if (_cateColorID != value)
                {
                    _cateColorID = value;
                    RaisePropertyChanged(() => CateColorID);
                }
            }
        }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    RaisePropertyChanged(() => Color);
                }
            }
        }

        public ToDoCategory(string name,int colorID)
        {
            this.CateName = name;
            this.CateColorID = colorID;
        }

        public ToDoCategory()
        {

        }
    }
}
