using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using JP.Utils.Network;
using JP.Utils.Debug;
using JP.Utils.Data;
using Newtonsoft.Json.Linq;

namespace MyerList.Helper
{
    public class PostHelper
    {
        public static string GetHashingCode()
        {
            //TIMESTAMP
            DateTime timeStamp = new DateTime(1970, 1, 1);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //SHA1 FUCK HASH
            MD5 md5 = MD5.Create();
            string requsetCode = NetworkHelper.GetMd5Hash(md5, unixTimestamp.ToString());
            return NetworkHelper.GetMd5Hash(md5, "1ca2bcdb6fe26c8f50480a02c6439f24" + requsetCode);
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
                var param = new Dictionary<string, string>()
                {
                    {"email",email},
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.UserCheckExist, new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
                return false;
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
                var md5 = MD5.Create();
                var ps = NetworkHelper.GetMd5Hash(md5, password); //把密码MD5加密

                var param = new Dictionary<string, string>()
                {
                    {"email",email},
                    {"password",ps},
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.UserRegisterUri, new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
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
                var param = new Dictionary<string, string>
                {
                    {"email",email }
                };

                HttpClient client = new HttpClient();
                var message = await client.PostAsync(UrlHelper.UserGetSalt, new FormUrlEncodedContent(param));
                if (message.IsSuccessStatusCode)
                {
                    var content = await message.Content.ReadAsStringAsync();
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
                var task = ExceptionHelper.WriteRecord(e);
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
                var md5 = MD5.Create();
                var ps = NetworkHelper.GetMd5Hash(md5, password); //把密码MD5加密,这是数据库存的密码

                var pss = NetworkHelper.GetMd5Hash(md5, ps + salt); //加密后的密码跟盐串联再MD5加密

                var param = new Dictionary<string, string>()
                {
                    {"email",email},
                    {"password",pss},
                };
                Debug.WriteLine(pss);

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.UserLoginUri, new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
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
        public async static Task<string> AddSchedule(string sid, string content, string isdone, string cate)
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    {"sid",sid},
                    {"time",DateTime.Now.ToString()},
                    {"content",content},
                    {"isdone",isdone},
                    {"cate",cate }
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleAddUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);
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
        public async static Task<bool> UpdateContent(string id, string content, int cate = 0)
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    {"content",content},
                    {"id",id},
                    {"cate",cate.ToString() }
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleUpdateUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
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
                var param = new Dictionary<string, string>()
                {
                    {"isdone",isdone},
                    {"id",id}
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleFinishUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
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
                var param = new Dictionary<string, string>()
                {
                    {"id",id}
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleDeleteUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
                return false;
            }

        }

        /// <summary>
        /// 查找我的所有日程
        /// </summary>
        /// <param name="sid">用户ID</param>
        /// <returns>返回JSON</returns>
        public async static Task<string> GetMySchedules(string sid)
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    {"sid",sid}
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleGetUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
                return null;
            }

        }

        public async static Task<string> GetMyOrder(string sid)
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    {"sid",sid}
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleGetOrderUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
                return null;
            }

        }

        public async static Task<bool> SetMyOrder(string sid, string order)
        {
            try
            {
                var param = new Dictionary<string, string>()
                {
                    {"sid",sid},
                    {"order",order}
                };

                HttpClient client = new HttpClient();
                var messsage = await client.PostAsync(UrlHelper.ScheduleSetOrderUri + "sid=" + LocalSettingHelper.GetValue("sid") + "&access_token=" + LocalSettingHelper.GetValue("access_token"), new FormUrlEncodedContent(param));
                if (messsage.IsSuccessStatusCode)
                {
                    var response = await messsage.Content.ReadAsStringAsync();
                    if (String.IsNullOrEmpty(response))
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
                var task = ExceptionHelper.WriteRecord(e);
                return false;
            }

        }

        public async static Task<string> GetCateInfo()
        {
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(UrlHelper.UserGetCateUri + $"sid={LocalSettingHelper.GetValue("sid")}&access_token={LocalSettingHelper.GetValue("access_token")}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

