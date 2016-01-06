using GalaSoft.MvvmLight;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using MyerList.Helper;
using MyerList.Model;
using MyerListUWP.Common;
using MyerListUWP.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace MyerListUWP.ViewModel
{
    public class CategoryViewModel:ViewModelBase
    {
        private ObservableCollection<ToDoCategory> _categories;
        public ObservableCollection<ToDoCategory> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    RaisePropertyChanged(() => Categories);
                }
            }
        }

        public CategoryViewModel()
        {
            Categories = new ObservableCollection<ToDoCategory>();
            App.CurrentCateVM = this;
        }

        public async Task<bool> SyncCateAsync()
        {
            try
            {
                var response = await PostHelper.GetCateInfo();
                if (response != null)
                {
                    var json = JsonObject.Parse(response);
                    var ok = JsonParser.GetBooleanFromJsonObj(json, "isSuccessed");
                    if (ok)
                    {
                        JsonArray array = JsonArray.Parse(json["Cate_Info"].GetString());
                        foreach (var item in array)
                        {
                            ToDoCategory newCate = new ToDoCategory();
                            newCate.CateName = JsonParser.GetStringFromJsonObj(item, "CateName");
                            newCate.CateColorID = (int)JsonParser.GetNumberFromJsonObj(item, "CateColor");

                            Categories.Add(newCate);
                        }
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<ObservableCollection<ToDoCategory>> GetCateInfoFromLocalAsync()
        {
            var cates= await SerializerHelper.DeserializeFromJsonByFileName<ObservableCollection<ToDoCategory>>(SerializerFileNames.CategoryFileName);
            return cates;
        }
    }
}
