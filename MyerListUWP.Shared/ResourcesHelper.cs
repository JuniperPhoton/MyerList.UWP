using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace MyerListShared
{
    public class ResourcesHelper
    {
        private static ResourceLoader _loader = new ResourceLoader();

        /// <summary>
        /// 获取 Strings 里的值，支持多语言
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetResString(string key)
        {
            return _loader.GetString(key);
        }
    }
}
