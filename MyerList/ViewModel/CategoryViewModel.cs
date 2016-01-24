using GalaSoft.MvvmLight;
using JP.Utils.Data;
using JP.Utils.Data.Json;
using JP.Utils.Debug;
using JP.Utils.UI;
using MyerList.Helper;
using MyerList.Model;
using MyerListUWP.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml.Media;

namespace MyerListUWP.ViewModel
{
    /// <summary>
    /// 表示类别
    /// 除了自定义/默认的，还有所有/已删除/自定义
    /// 所有ID：0
    /// 已删除ID：-1
    /// 自定义ID：-2
    /// </summary>
    public class CategoryViewModel : ViewModelBase
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
                    RaisePropertyChanged(() => CatesToModify);
                    RaisePropertyChanged(() => CatesToAdd);
                }
            }
        }

        public ObservableCollection<ToDoCategory> CatesToModify
        {
            get
            {
                var list = from e in Categories where (e.CateColorID > 0) select e;
                var listToReturn = new ObservableCollection<ToDoCategory>();
                list.ToList().ForEach(s => listToReturn.Add(s));
                return listToReturn;
            }
        }

        public ObservableCollection<ToDoCategory> CatesToAdd
        {
            get
            {
                var list = from e in Categories where (e.CateColorID >= 0) select e;
                var listToReturn = new ObservableCollection<ToDoCategory>();
                list.ToList().ForEach(s => listToReturn.Add(s));
                return listToReturn;
            }
        }


        public CategoryViewModel()
        {
            Categories = new ObservableCollection<ToDoCategory>();
        }

        public async Task Initial(LoginMode mode)
        {
            Categories = await RestoreCacheButDefaultList();
            //已经登陆过的了
            if (mode != LoginMode.OfflineMode && !App.IsNoNetwork)
            {
                var isGot = await GetLatestCates();
                if (!isGot)
                {
                    Categories = await RestoreCacheButDefaultList();
                    await SerializerHelper.SerializerToJson<ObservableCollection<ToDoCategory>>(Categories, SerializerFileNames.CategoryFileName);
                }
            }
            Categories.Insert(0, new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateAll"), CateColor = App.Current.Resources["MyerListBlue"] as SolidColorBrush ,CateColorID=0});
            Categories.Add(new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateDelete"), CateColor = App.Current.Resources["DeletedColor"] as SolidColorBrush ,CateColorID=-1});
        }

        private async Task<ObservableCollection<ToDoCategory>> RestoreCacheButDefaultList()
        {
            var cacheList = await SerializerHelper.DeserializeFromJsonByFileName
                <ObservableCollection<ToDoCategory>>(SerializerFileNames.CategoryFileName);
            if (cacheList != null)
            {
                if (cacheList.Count >= 3)
                {
                    return cacheList;
                }
            }
            var defaultList = await GenerateListAsync(AppSettings.DefaultCateJsonString);
            return defaultList;
        }

        public async Task<bool> GetLatestCates()
        {
            try
            {
                var response = await PostHelper.GetCateInfo();
                if (response == null) throw new ArgumentNullException();

                var respJson = JsonObject.Parse(response);
                var isSuccess = JsonParser.GetBooleanFromJsonObj(respJson, "isSuccessed");
                if (!isSuccess) throw new ArgumentException();

                var cateObj = JsonParser.GetStringFromJsonObj(respJson, "Cate_Info");
                var list = await GenerateListAsync(cateObj.ToString());
                if (list == null) throw new ArgumentNullException();

                Categories.Clear();

                foreach (var item in list)
                {
                    Categories.Add(item);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<ObservableCollection<ToDoCategory>> GenerateListAsync(string cateJson)
        {
            var list = new ObservableCollection<ToDoCategory>();
            try
            {
                var cateObj = JsonObject.Parse(cateJson);
                var isModify = JsonParser.GetBooleanFromJsonObj(cateObj, "modified");

                if (!isModify) throw new ArgumentException();

                var cateArray = JsonParser.GetJsonArrayFromJsonObj(cateObj, "cates");
                foreach (var item in cateArray)
                {
                    ToDoCategory newCate = new ToDoCategory();
                    newCate.CateName = JsonParser.GetStringFromJsonObj(item, "name");
                    var colorStr = JsonParser.GetStringFromJsonObj(item, "color");
                    newCate.CateColor = new SolidColorBrush(ColorConverter.Hex2Color(colorStr.Replace("#FF", string.Empty)));
                    newCate.CateColorID = (int)JsonParser.GetNumberFromJsonObj(item, "id");
                    list.Add(newCate);
                }
                return list;
            }
            catch (Exception e)
            {
                await ExceptionHelper.WriteRecordAsync(e, nameof(CategoryViewModel), nameof(GenerateListAsync));
                return null;
            }
        }

    }
}
