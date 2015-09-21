using GalaSoft.MvvmLight;
using MyerList.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    return ResourcesHelper.GetString("DefaultCateName");
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

        private int _cateColor;
        public int CateColor
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
                }
            }
        }

        public ToDoCategory(string name,int color)
        {
            this.CateName = name;
            this.CateColor = color;
        }

        public ToDoCategory()
        {

        }

    }
}
