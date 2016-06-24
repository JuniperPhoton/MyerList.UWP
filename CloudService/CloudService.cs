using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using JP.Utils.Network;
using JP.Utils.Debug;
using JP.Utils.Data;
using JP.API;
using System.Threading;
using Windows.Data.Json;
using JP.Utils.Data.Json;

namespace MyerList.Helper
{
    public static class CloudService
    {
        private static void ParseResult(this CommonRespMsg result)
        {
            JsonObject resultObj;
            var ok = JsonObject.TryParse(result.JsonSrc, out resultObj);
            if (ok)
            {
                var errorCode = JsonParser.GetStringFromJsonObj(resultObj, "error_code");
                var errorMsg = JsonParser.GetStringFromJsonObj(resultObj, "error_message");

                if (!string.IsNullOrEmpty(errorCode))
                {
                    if (errorCode != "0")
                    {
                        result.IsSuccessful = false;
                    }
                    result.ErrorCode = int.Parse(errorCode);
                    result.ErrorMsg = errorMsg;
                }
                else result.IsSuccessful = false;
            }
            else result.IsSuccessful = false;
        }

        private static List<KeyValuePair<string, string>> GetDefaultParam()
        {
            var param = new List<KeyValuePair<string, string>>();
            param.Add(new KeyValuePair<string, string>("a", new Random().Next().ToString()));
            return param;
        }

        /// <summary>
        /// Check if email input is existed
        /// </summary>
        /// <param name="email"></param>
        /// <returns>return the existance of email</returns>
        public async static Task<bool> CheckEmailExistAsync(string email)
        {
            var param = GetDefaultParam();
            param.Add(new KeyValuePair<string, string>("email", email));

            CancellationTokenSource cts = new CancellationTokenSource();
            var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.UserCheckExist, param, cts.Token);
            result.ParseResult();
            if (result.IsSuccessful)
            {
                var jsonObj = JsonObject.Parse(result.JsonSrc);
                var isExist = JsonParser.GetBooleanFromJsonObj(jsonObj, "isExist", false);
                return isExist;
            }
            else return false;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="email">电子邮件</param>
        /// <param name="password">密码</param>
        /// <returns>成功返回盐</returns> 
        public async static Task<string> RegisterAsync(string email, string password)
        {
            try
            {
                var ps = MD5.GetMd5String(password); //把密码MD5加密

                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));
                param.Add(new KeyValuePair<string, string>("password", ps));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.UserRegisterUri, param, cts.Token);
                result.ParseResult();
                if (result.IsSuccessful)
                {
                    JsonObject obj = JsonObject.Parse(result.JsonSrc);
                    var userInfo = JsonParser.GetJsonObjFromJsonObj(obj, "UserInfo");
                    var salt = JsonParser.GetStringFromJsonObj(userInfo, "Salt");
                    return salt;
                }
                else
                {
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        /// <summary>
        /// 获盐
        /// </summary>
        /// <param name="email">用户Email</param>
        /// <returns></returns>
        public async static Task<string> GetSaltAsync(string email)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.UserGetSalt, param, cts.Token);
                result.ParseResult();
                if (result.IsSuccessful)
                {
                    JsonObject obj = JsonObject.Parse(result.JsonSrc);
                    var salt = JsonParser.GetStringFromJsonObj(obj, "Salt");
                    return salt;
                }
                else return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="email">电子邮件</param>
        /// <param name="password">原始密码</param>
        /// <returns>成功返回True</returns>
        public async static Task<bool> LoginAsync(string email, string password, string salt)
        {
            try
            {
                var ps = MD5.GetMd5String(password); //把密码MD5加密,这是数据库存的密码
                var psplussalt = MD5.GetMd5String(ps + salt); //加密后的密码跟盐串联再MD5加密

                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));
                param.Add(new KeyValuePair<string, string>("password", psplussalt));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.UserLoginUri, param, cts.Token);
                result.ParseResult();
                if (result.IsSuccessful)
                {
                    JsonObject obj = JsonObject.Parse(result.JsonSrc);
                    var userObj = JsonParser.GetJsonObjFromJsonObj(obj, "UserInfo");
                    var sid = JsonParser.GetStringFromJsonObj(userObj, "sid");
                    var token = JsonParser.GetStringFromJsonObj(userObj, "access_token");

                    LocalSettingHelper.AddValue("sid", sid);
                    LocalSettingHelper.AddValue("access_token", token);
                    LocalSettingHelper.AddValue("email", email);
                    LocalSettingHelper.AddValue("password", password);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加日程
        /// </summary>
        /// <param name="sid">用户ID</param>
        /// <param name="content">内容</param>
        /// <param name="isdone">是否完成 0未完成 1完成</param>
        /// <returns>返回JSON数据</returns>
        public async static Task<CommonRespMsg> AddToDoAsync(string content, string isdone, string cate)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));
                param.Add(new KeyValuePair<string, string>("time", DateTime.Now.ToString()));
                param.Add(new KeyValuePair<string, string>("content", content));
                param.Add(new KeyValuePair<string, string>("isdone", isdone));
                param.Add(new KeyValuePair<string, string>("cate", cate));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var task = HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleAddUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                var result = await task;
                result.ParseResult();
                return result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新日程内容
        /// </summary>
        /// <param name="id">日程ID</param>
        /// <param name="content">更新后的内容</param>
        /// <param name="cate">类别</param>
        /// <returns>成功返回True</returns>
        public async static Task<bool> UpdateToDoContentAsync(string id, string content, string time, int cate = 0)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("content", content));
                param.Add(new KeyValuePair<string, string>("id", id));
                param.Add(new KeyValuePair<string, string>("cate", cate.ToString()));
                param.Add(new KeyValuePair<string, string>("time", time));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleUpdateUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result.IsSuccessful;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        /// <summary>
        /// Toggle是否完成
        /// </summary>
        /// <param name="id">日程ID</param>
        /// <param name="isdone">是否完成</param>
        /// <returns></returns>
        public async static Task<bool> FinishToDoAsync(string id, string isdone)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("id", id));
                param.Add(new KeyValuePair<string, string>("isdone", isdone));

                HttpClient client = new HttpClient();
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleFinishUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result.IsSuccessful;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        /// <summary>
        /// 删除日程
        /// </summary>
        /// <param name="id">日程ID</param>
        /// <returns></returns>
        public async static Task<bool> DeleteToDoAsync(string id)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("id", id));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleDeleteUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result.IsSuccessful;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        /// <summary>
        /// 查找我的所有日程
        /// </summary>
        /// <param name="sid">用户ID</param>
        /// <returns>返回JSON</returns>
        public async static Task<CommonRespMsg> GetMyToDosAsync()
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleGetUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }

        public async static Task<string> GetMyOrderAsync()
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));

                HttpClient client = new HttpClient();
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleGetOrderUri +
                    "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                if (result.IsSuccessful)
                {
                    var obj = JsonObject.Parse(result.JsonSrc);
                    var array = JsonParser.GetJsonArrayFromJsonObj(obj, "OrderList");
                    var firstObj = array.GetObjectAt(0);
                    var listOrder = JsonParser.GetStringFromJsonObj(firstObj, "list_order");
                    return listOrder;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }

        public async static Task<bool> UpdateAllOrderAsync(string order)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));
                param.Add(new KeyValuePair<string, string>("order", order));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.ScheduleSetOrderUri +
                    "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result.IsSuccessful;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        public async static Task<CommonRespMsg> GetCateInfoAsync()
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendGetRequestAsync(UrlHelper.UserGetCateUri + $"sid={UrlHelper.SID}&access_token={UrlHelper.AccessToken}&a={new Random().Next()}", cts.Token);
                result.ParseResult();
                return result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static Task<bool> UpdateCateInfoAsync(string content)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));
                param.Add(new KeyValuePair<string, string>("access_token", UrlHelper.AccessToken));
                param.Add(new KeyValuePair<string, string>("cate_info", content));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await HttpRequestSender.SendPostRequestAsync(UrlHelper.UserUpdateCateUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                result.ParseResult();
                return result.IsSuccessful;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

