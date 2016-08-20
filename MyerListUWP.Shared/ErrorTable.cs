using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyerListShared
{
    //common error define
    //   define('API_ERROR_DATABASE_ERROR', 100);
    //   define('API_ERROR_ACTION_NOTEXIST', 101);
    //   define('API_ERROR_ACCESS_TOKEN_INVAID', 102);

    //   //user
    //   define('API_ERROR_USER_NOTEXIST', 200);
    //   define('API_ERROR_PARM_LACK', 202);
    //   define('API_ERROR_USER_ALEADY_EXIST', 203);
    //   define('API_ERROR_USER_NOT_EXIST', 204);
    //   define('API_ERROR_CHECK_USER_NAME_OR_PWD',205);

    //   //schedule
    //   define('API_ERROR_LACK_PARAM', 300);
    //   define('API_ERROR_SCHEDULE_NOT_EXIST', 301);
    public enum APIErrors
    {
        API_ERROR_DATABASE_ERROR = 101,
        API_ERROR_ACTION_NOTEXIST,
        API_ERROR_ACCESS_TOKEN_INVAID,
        API_ERROR_USER_NOTEXIST = 200,
        API_ERROR_PARM_LACK,
        API_ERROR_USER_ALEADY_EXIST,
        API_ERROR_USER_NOT_EXIST,
        API_ERROR_CHECK_USER_NAME_OR_PWD,
        API_ERROR_LACK_PARAM = 300,
        API_ERROR_SCHEDULE_NOT_EXIST
    }

    public static class ErrorUtils
    {
        public static string GetUserMsgFromErrorCode(int code)
        {
            switch(code)
            {
                case 200:
                    {
                        return ResourcesHelper.GetResString("ErrorAccountNotExist");
                    };
                case 202:
                    {
                        return ResourcesHelper.GetResString("ErrorUserExist");
                    };
                case 205:
                    {
                        return ResourcesHelper.GetResString("ErrorLoginFail");
                    };
                default:
                    {
                        return ResourcesHelper.GetResString("ErrorDatabase");
                    }
            }
        }
    }
}
