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
        /// <summary>
        /// 所有用于展示在抽屉的类别
        /// </summary>
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

        /// <summary>
        /// 个性化里出现的类别
        /// </summary>
        private ObservableCollection<ToDoCategory> _catesToModify;
        public ObservableCollection<ToDoCategory> CatesToModify
        {
            get
            {
                return _catesToModify;
            }
            set
            {
                _catesToModify = value;
                RaisePropertyChanged(() => CatesToModify);
            }
        }

        /// <summary>
        /// 出现在添加/修改里的类别
        /// </summary>
        public ObservableCollection<ToDoCategory> _catesToAdd;
        public ObservableCollection<ToDoCategory> CatesToAdd
        {
            get
            {
                return _catesToAdd;
            }
            set
            {
                if(_catesToAdd!=value)
                {
                    _catesToAdd = value;
                    RaisePropertyChanged(() => CatesToAdd);
                }
            }
        }

        public CategoryViewModel()
        {
            Categories = new ObservableCollection<ToDoCategory>();
        }

        public void UpdateCatesToAdd()
        {
            var list = from e in Categories where (e.CateColorID >= 0) select e;
            var listToReturn = new ObservableCollection<ToDoCategory>();
            list.ToList().ForEach(s => listToReturn.Add(s));
            CatesToAdd = listToReturn;
        }

        public void UpdateCateToModify()
        {
            var list = from e in Categories where (e.CateColorID > 0) select e;
            var cateList = new ObservableCollection<ToDoCategory>();
            foreach (var item in list)
            {
                var cate = new ToDoCategory()
                {
                    CateColorID = item.CateColorID,
                    CateName = item.CateName,
                    CateColor = item.CateColor,
                };
                cateList.Add(cate);
            }
            CatesToModify = cateList;
        }

        public async Task<bool> SaveCatesToModify()
        {
            var currentMaxID = CreateNewID();
            var array = new JsonArray();
            foreach(var item in CatesToModify)
            {
                var obj = new JsonObject();
                obj.Add("name", JsonValue.CreateStringValue(item.CateName));
                obj.Add("color", JsonValue.CreateStringValue(item.CateColor.Color.ToString()));
                obj.Add("id", JsonValue.CreateNumberValue(item.CateColorID));
                array.Add(obj);
            }
            var arrayString = array.ToString();
            var cateString = AppSettings.ModifiedCateJsonStringFore + arrayString + "}";

            try
            {
                if (!App.IsInOfflineMode)
                {
                    var isOK = await CloudService.UpdateCateInfo(cateString);
                    if (!isOK) throw new ArgumentException();
                }

                Categories = CatesToModify;
                await SerializerHelper.SerializerToJson<ObservableCollection<ToDoCategory>>(Categories, SerializerFileNames.CategoryFileName);
                Categories.Insert(0, new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateAll"), CateColor = App.Current.Resources["MyerListBlue"] as SolidColorBrush, CateColorID = 0 });
                Categories.Add(new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateDelete"), CateColor = App.Current.Resources["DeletedColor"] as SolidColorBrush, CateColorID = -1 });
                UpdateCatesToAdd();
                App.MainVM.RefreshCate();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public int CreateNewID()
        {
            var ids = from e in Categories select e.CateColorID;
            var max = ids.Max();
            return max++;
        }

        public async Task Refresh(LoginMode mode)
        {
            if(Categories.Count==0)
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
            Categories.ToList().ForEach(s => s.UpdateColor());
            Categories.Insert(0, new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateAll"), CateColor = App.Current.Resources["MyerListBlue"] as SolidColorBrush ,CateColorID=0});
            Categories.Add(new ToDoCategory() { CateName = ResourcesHelper.GetResString("CateDelete"), CateColor = App.Current.Resources["DeletedColor"] as SolidColorBrush ,CateColorID=-1});
            UpdateCateToModify();
            UpdateCatesToAdd();
            App.MainVM.RefreshCate();
        }

        /// <summary>
        /// 从本地缓存还原列表，如果不存在缓存，则还原默认列表
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 从服务器同步列表
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetLatestCates()
        {
            try
            {
                var result = await CloudService.GetCateInfo();
                if (result == null) throw new ArgumentNullException();

                var respJson = JsonObject.Parse(result.JsonSrc);
                var isSuccess = JsonParser.GetBooleanFromJsonObj(respJson, "isSuccessed");
                if (!isSuccess) throw new ArgumentException();

                var cateObj = JsonParser.GetStringFromJsonObj(respJson, "Cate_Info");
                var list = await GenerateListAsync(cateObj.ToString());
                if (list == null) throw new ArgumentNullException();

                Categories = list;

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
                    newCate.CateColor = new SolidColorBrush(ColorConverter.HexToColor(colorStr).Value);
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
