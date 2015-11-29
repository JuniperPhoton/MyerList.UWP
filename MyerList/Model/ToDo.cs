using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using ChaoFunctionRT;
using JP.Utils.Debug;
using Windows.ApplicationModel.Resources;
using MyerList.Helper;

namespace MyerList.Model
{
    public class ToDo:ViewModelBase
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
                    if (_category == 5) _category = 0;
                }
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
                if(_id!=value)
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
                if(_sid!=value)
                    _sid = value;
                RaisePropertyChanged(()=>SID);
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
                if(_content!=value)
                    _content = value;
                RaisePropertyChanged(()=>Content);
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


        private int _order;
        public int Order
        {
            get
            {
                return _order;
            }
            set
            {
                if(_order!=value)
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
                if(_isdone!=value)
                    _isdone = value;
                RaisePropertyChanged(()=>IsDone);
            }
        }

        public ToDo()
        {
            IsDone = false;
            Content = "";
            Order = 0;
            Category = 0;
        }

        public static ObservableCollection<ToDo> SetOrderByString(ObservableCollection<ToDo> orisches, string orderString)
        {
            try
            {
                if (orderString != "0")
                {
                    var order_list = orderString.Split(',');
                    ObservableCollection<ToDo> temp = new ObservableCollection<ToDo>();
                    foreach (var orderID in order_list)
                    {
                        if (orderID == "" || orderID==" ")
                        {
                            continue;
                        }
                        var currentSche = orisches.ToList().Find((sche) =>
                        {
                            if (sche.ID == orderID)
                            {
                                return true;
                            }
                            else return false;
                        });
                        if (currentSche != null)
                        {
                            temp.Add(currentSche);
                            orisches.Remove(currentSche);
                        }
                    }
                    foreach(var item in orisches)
                    {
                        temp.Add(item);
                    }
                    return temp;
                }
                else return orisches;
            
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                return orisches;
            }
           
        }

        public static ObservableCollection<ToDo> ParseJsonToObs(string jsontext)
        {
            try
            {
                ObservableCollection<ToDo> schedules = new ObservableCollection<ToDo>();

                JObject job = JObject.Parse(jsontext);
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
                        else newSchedule.CreateTime = ResourcesHelper.GetString("UnknownTime");

                        newSchedule.ID = (string)sch["id"];
                        newSchedule.SID = (string)sch["sid"];
                        newSchedule.Content = (string)sch["content"];
                        newSchedule.IsDone = (string)sch["isdone"] != "0";
                        newSchedule.Category = (int)sch["cate"];

                        schedules.Add(newSchedule);
                    }
                return schedules;
            }
            catch(Exception e)
            {
                var task=ExceptionHelper.WriteRecord(e);
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
                if (schedules.Count == 0 || schedules==null)
                {
                    return "0";
                }
                StringBuilder sb = new StringBuilder();
                foreach (var sche in schedules)
                {
                    if(sche!=null) sb.Append(sche.ID + ",");
                }

                return sb.ToString();
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                return null;
            }
        }
    }
}

