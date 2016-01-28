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
        public const string DOMAIN = "juniperphoton.net";
        public const string PROTOCOL ="http";

        public static string UserCheckExist = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/CheckUserExist/v1?";
        public static string UserRegisterUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/Register/v1?";
        public static string UserLoginUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/Login/v1?";
        public static string UserGetSalt = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/GetSalt/v1";
        public static string ScheduleAddUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/AddSchedule/v1?";
        public static string ScheduleUpdateUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/UpdateContent/v1?";
        public static string ScheduleFinishUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/FinishSchedule/v1?";
        public static string ScheduleDeleteUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/DeleteSchedule/v1?";
        public static string ScheduleGetUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/GetMySchedules/v1?";
        public static string ScheduleGetOrderUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/GetMyOrder/v1?";
        public static string ScheduleSetOrderUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/Schedule/SetMyOrder/v1?";
        public static string UserGetCateUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/GetCateInfo/v1?";
        public static string UserUpdateCateUri = $"{PROTOCOL}://" + DOMAIN + "/schedule/User/UpdateCateInfo/v1?";
        #endregion

    }
}
