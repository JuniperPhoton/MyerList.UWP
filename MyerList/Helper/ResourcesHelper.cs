using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace MyerList.Helper
{
    public class ResourcesHelper
    {
        private static ResourceLoader _loader = new ResourceLoader();
        public static string GetString(string key)
        {
            return _loader.GetString(key);
        }
    }
}
