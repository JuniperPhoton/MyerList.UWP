using GalaSoft.MvvmLight;
using JP.Utils.Data.Json;
using JP.Utils.UI;
using MyerList.Helper;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Windows.Data.Json;
using Windows.UI.Xaml.Media;

namespace MyerList.Model
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

        private SolidColorBrush _cateColor;
        [IgnoreDataMember]
        public SolidColorBrush CateColor
        {
            get
            {
                return _cateColor;
            }
            set
            {
                if (_cateColor != value)
                {
                    _cateColor = value;
                    RaisePropertyChanged(() => CateColor);
                    CateColorString = value.Color.ToString();
                }
            }
        }

        public string CateColorString { get; set; }

        public ToDoCategory(string name,int colorID):this()
        {
            this.CateName = name;
            this.CateColorID = colorID;
        }

        public ToDoCategory()
        {
            
        }

        public void UpdateColor()
        {
            if(!string.IsNullOrEmpty(CateColorString))
            {
                this.CateColor = new SolidColorBrush(ColorConverter.Hex2Color(CateColorString));
            }
        }

        public static ObservableCollection<ToDoCategory> GenerateList(string jsonStr)
        {
            ObservableCollection<ToDoCategory> list = new ObservableCollection<ToDoCategory>();
            var jsonObj = JsonObject.Parse(jsonStr);
            var isModify = JsonParser.GetBooleanFromJsonObj(jsonObj, "modified");
            var array = JsonParser.GetJsonArrayFromJsonObj(jsonObj, "cates");
            foreach(var item in array)
            {
                var name = JsonParser.GetStringFromJsonObj(item, "name");
                var color = JsonParser.GetStringFromJsonObj(item, "color");

                var newCate = new ToDoCategory();
                newCate.CateName = name;
                newCate.CateColor = new SolidColorBrush(ColorConverter.Hex2Color(color.Replace("#FF","#")));
                list.Add(newCate);
            }
            return list;
        }
    }
}
