using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using JP.Utils.Debug;
using MyerList.Helper;
using Windows.UI.Xaml.Media;
using MyerListUWP;
using MyerList.ViewModel;
using Windows.UI.Xaml;
using Newtonsoft.Json.Linq;
using MyerListCustomControl;

namespace MyerList.Model
{
    public class ToDo : ViewModelBase
    {
        private int _category;
        public int Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    RaisePropertyChanged(() => Category);
                    RaisePropertyChanged(() => CateColor);
                }
            }
        }

        public SolidColorBrush CateColor
        {
            get
            {
                if (App.MainVM.CateVM != null)
                {
                    if (App.MainVM.CateVM.Categories != null)
                    {
                        var color = (from e in App.MainVM.CateVM.Categories where e.CateColorID == Category select e.CateColor).FirstOrDefault();
                        if (color != null)
                        {
                            return color;
                        }
                        else return (App.Current.Resources["MyerListBlue"] as SolidColorBrush);
                    }
                }
                return (App.Current.Resources["MyerListBlue"] as SolidColorBrush);
            }
        }

        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                    _id = value;
                RaisePropertyChanged(() => ID);
            }
        }

        private string _sid;
        public string SID
        {
            get
            {
                return _sid;
            }
            set
            {
                if (_sid != value)
                    _sid = value;
                RaisePropertyChanged(() => SID);
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != value)
                    _content = value;
                RaisePropertyChanged(() => Content);
            }
        }

        private string _createTime;
        public string CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (_createTime != value)
                {
                    _createTime = value;
                    RaisePropertyChanged(() => CreateTime);
                }
            }
        }

        private Visibility _shoDoneLine;
        public Visibility ShowDoneLine
        {
            get
            {
                return _shoDoneLine;
            }
            set
            {
                if (_shoDoneLine != value)
                {
                    _shoDoneLine = value;
                    RaisePropertyChanged(() => ShowDoneLine);
                }
            }
        }


        private int _order;
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    RaisePropertyChanged(() => Order);
                }
            }
        }

        private bool _isdone;
        public bool IsDone
        {
            get
            {
                return _isdone;
            }
            set
            {
                if (_isdone != value)
                {
                    _isdone = value;
                    RaisePropertyChanged(() => IsDone);
                    ShowDoneLine = IsDone ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public ToDo()
        {
            IsDone = false;
            Content = "";
            Order = 0;
            Category = 0;
            ShowDoneLine = Visibility.Collapsed;
        }

        public static ObservableCollection<ToDo> GetSortedList(ObservableCollection<ToDo> originalList, string orderString)
        {
            try
            {
                if (orderString != "0")
                {
                    var sortedIDs = orderString.Split(',');
                    var tempToDoList = new ObservableCollection<ToDo>();
                    foreach (var id in sortedIDs)
                    {
                        if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                        {
                            continue;
                        }
                        var foundToDo = originalList.ToList().Find((s) =>
                        {
                            if (s.ID == id)
                            {
                                return true;
                            }
                            else return false;
                        });
                        if (foundToDo != null)
                        {
                            tempToDoList.Add(foundToDo);
                            originalList.Remove(foundToDo);
                        }
                    }

                    //剩下的还没有排序的
                    foreach (var item in originalList)
                    {
                        tempToDoList.Add(item);
                    }
                    return tempToDoList;
                }
                else return originalList;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return originalList;
            }
        }

        public static ObservableCollection<ToDo> ParseJsonToObs(string rawJson)
        {
            try
            {
                ObservableCollection<ToDo> schedules = new ObservableCollection<ToDo>();

                JObject job = JObject.Parse(rawJson);
                JArray array = job["ScheduleInfo"] as JArray;
                if (array != null)
                    foreach (var sch in array)
                    {
                        ToDo newSchedule = new ToDo();

                        var timeString = (string)sch["time"];
                        DateTime time;
                        if (DateTime.TryParse(timeString, out time))
                        {
                            newSchedule.CreateTime = time.ToString();
                        }
                        else newSchedule.CreateTime = ResourcesHelper.GetResString("UnknownTime");

                        newSchedule.ID = (string)sch["id"];
                        newSchedule.SID = (string)sch["sid"];
                        newSchedule.Content = (string)sch["content"];
                        newSchedule.IsDone = (string)sch["isdone"] != "0";
                        newSchedule.Category = (int)sch["cate"];

                        schedules.Add(newSchedule);
                    }
                return schedules;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }

        }

        public static ToDo ParseJsonTo(string jsontext)
        {
            JObject job = JObject.Parse(jsontext);
            JObject info = job["ScheduleInfo"] as JObject;

            if (info == null) return null;

            ToDo newSchedule = new ToDo();

            newSchedule.ID = (string)info["id"];
            newSchedule.SID = (string)info["sid"];
            newSchedule.Category = (int)info["cate"];

            DateTime time;
            DateTime.TryParse((string)info["time"], out time);

            newSchedule.Content = (string)info["content"];
            newSchedule.IsDone = (int)info["isdone"] != 0;

            return newSchedule;
        }

        public static string GetCurrentOrderString(ObservableCollection<ToDo> schedules)
        {
            try
            {
                if (schedules.Count == 0 || schedules == null)
                {
                    return "0";
                }
                StringBuilder sb = new StringBuilder();
                foreach (var sche in schedules)
                {
                    if (sche != null) sb.Append(sche.ID + ",");
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }
    }
}

