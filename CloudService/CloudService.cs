using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using JP.Utils.Network;
using JP.Utils.Debug;
using JP.Utils.Data;
using Newtonsoft.Json.Linq;
using JP.API;
using System.Threading;

namespace MyerList.Helper
{
    public class CloudService
    {
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
        public async static Task<bool> CheckExist(string email)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.UserCheckExist, param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"])
                    {
                        if ((bool)job["isExist"])
                        {
                            return true;
                        }
                        else return false;
                    }
                    else throw new Exception();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="email">电子邮件</param>
        /// <param name="password">密码</param>
        /// <returns>成功返回盐</returns>
        public async static Task<string> Register(string email, string password)
        {
            try
            {
                var ps = MD5.GetMd5String(password); //把密码MD5加密

                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));
                param.Add(new KeyValuePair<string, string>("password", ps));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.UserRegisterUri, param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (String.IsNullOrEmpty(response))
                    {
                        return null;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"])
                    {
                        return (string)job["UserInfo"]["Salt"];
                    }
                    else return null;
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }

        /// <summary>
        /// 获盐
        /// </summary>
        /// <param name="email">用户Email</param>
        /// <returns></returns>
        public async static Task<string> GetSalt(string email)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var message = await APIHelper.SendPostRequestAsync(UrlHelper.UserGetSalt, param, cts.Token);
                if (message.IsSuccessful)
                {
                    var content = message.JsonSrc;
                    JObject job = JObject.Parse(content);
                    if ((bool)job["isSuccessed"])
                    {
                        return (string)job["Salt"];
                    }
                    else return null;
                }
                else return null;
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="email">电子邮件</param>
        /// <param name="password">原始密码</param>
        /// <returns>成功返回True</returns>
        public async static Task<bool> Login(string email, string password, string salt)
        {
            try
            {
                var ps = MD5.GetMd5String(password); //把密码MD5加密,这是数据库存的密码
                var psplussalt = MD5.GetMd5String(ps + salt); //加密后的密码跟盐串联再MD5加密

                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("email", email));
                param.Add(new KeyValuePair<string, string>("password", psplussalt));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.UserLoginUri, param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (String.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"])
                    {
                        LocalSettingHelper.AddValue("sid", (string)job["UserInfo"]["sid"]);
                        LocalSettingHelper.AddValue("access_token", (string)job["UserInfo"]["access_token"]);
                        LocalSettingHelper.AddValue("email", email);
                        LocalSettingHelper.AddValue("password", password);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        /// <summary>
        /// 添加日程
        /// </summary>
        /// <param name="sid">用户ID</param>
        /// <param name="content">内容</param>
        /// <param name="isdone">是否完成 0未完成 1完成</param>
        /// <returns>返回JSON数据</returns>
        public async static Task<string> AddSchedule(string content, string isdone, string cate)
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
                var task = APIHelper.SendPostRequestAsync(UrlHelper.ScheduleAddUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                var result = await task;
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return null;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return response;
                    else return null;
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return null;
            }
        }

        /// <summary>
        /// 更新日程内容
        /// </summary>
        /// <param name="id">日程ID</param>
        /// <param name="content">更新后的内容</param>
        /// <param name="cate">类别</param>
        /// <returns>成功返回True</returns>
        public async static Task<bool> UpdateContent(string id, string content, string time, int cate = 0)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("content", content));
                param.Add(new KeyValuePair<string, string>("id", id));
                param.Add(new KeyValuePair<string, string>("cate", cate.ToString()));
                param.Add(new KeyValuePair<string, string>("time", time));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleUpdateUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
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
        public async static Task<bool> FinishSchedule(string id, string isdone)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("id", id));
                param.Add(new KeyValuePair<string, string>("isdone", isdone));

                HttpClient client = new HttpClient();
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleFinishUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
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
        public async static Task<bool> DeleteSchedule(string id)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("id", id));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleDeleteUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
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
        public async static Task<string> GetMySchedules()
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleGetUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return null;
                    }
                    Debug.WriteLine(response);
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return response;
                    else return null;

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

        public async static Task<string> GetMyOrder()
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));

                HttpClient client = new HttpClient();
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleGetOrderUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return null;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"])
                    {
                        var orderString = (string)(((job["OrderList"] as JArray).First)["list_order"]);
                        if (orderString != null)
                        {
                            return orderString;
                        }
                        else return null;
                    }
                    else return null;

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

        public async static Task<bool> SetAllOrder(string order)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));
                param.Add(new KeyValuePair<string, string>("order", order));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.ScheduleSetOrderUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (result.IsSuccessful)
                {
                    var response = result.JsonSrc;
                    if (string.IsNullOrEmpty(response))
                    {
                        return false;
                    }
                    JObject job = JObject.Parse(response);
                    if ((bool)job["isSuccessed"]) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e);
                return false;
            }
        }

        public async static Task<string> GetCateInfo()
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var response = await APIHelper.SendGetRequestAsync(UrlHelper.UserGetCateUri + $"sid={UrlHelper.SID}&access_token={UrlHelper.AccessToken}&a={new Random().Next()}", cts.Token);
                return response.JsonSrc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static Task<bool> UpdateCateInfo(string content)
        {
            try
            {
                var param = GetDefaultParam();
                param.Add(new KeyValuePair<string, string>("sid", UrlHelper.SID));
                param.Add(new KeyValuePair<string, string>("access_token", UrlHelper.AccessToken));
                param.Add(new KeyValuePair<string, string>("cate_info", content));

                CancellationTokenSource cts = new CancellationTokenSource(10000);
                var result = await APIHelper.SendPostRequestAsync(UrlHelper.UserUpdateCateUri + "sid=" + UrlHelper.SID + "&access_token=" + UrlHelper.AccessToken,
                    param, cts.Token);
                if (!result.IsSuccessful) throw new ArgumentException();

                var json = result.JsonSrc;
                if (string.IsNullOrEmpty(json)) throw new ArgumentException();

                JObject job = JObject.Parse(json);
                if ((bool)job["isSuccessed"]) return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

