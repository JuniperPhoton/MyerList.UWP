using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyerList.Helper
{
    public static class UrlHelper
    {
        #region API_URI

        public static string domain = "juniperphoton.net";
        public static string UserCheckExist = "http://" + domain + "/schedule/User/CheckUserExist/v1?";
        public static string UserRegisterUri = "http://" + domain + "/schedule/User/Register/v1?";
        public static string UserLoginUri = "http://" + domain + "/schedule/User/Login/v1?";
        public static string UserGetSalt = "http://" + domain + "/schedule/User/GetSalt/v1";
        public static string ScheduleAddUri = "http://" + domain + "/schedule/Schedule/AddSchedule/v1?";
        public static string ScheduleUpdateUri = "http://" + domain + "/schedule/Schedule/UpdateContent/v1?";
        public static string ScheduleFinishUri = "http://" + domain + "/schedule/Schedule/FinishSchedule/v1?";
        public static string ScheduleDeleteUri = "http://" + domain + "/schedule/Schedule/DeleteSchedule/v1?";
        public static string ScheduleGetUri = "http://" + domain + "/schedule/Schedule/GetMySchedules/v1?";
        public static string ScheduleGetOrderUri = "http://" + domain + "/schedule/Schedule/GetMyOrder/v1?";
        public static string ScheduleSetOrderUri = "http://" + domain + "/schedule/Schedule/SetMyOrder/v1?";
        public static string UserGetCateUri = "http://" + domain + "/schedule/User/GetCateInfo/v1?";
        #endregion

    }
}
