using MyerListUWP;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace MyerList.Helper
{
    public class ResourcesHelper
    {
        private static ResourceLoader _loader = new ResourceLoader();
        public static string GetResString(string key)
        {
            return _loader.GetString(key);
        }

        public static string GetDicString(string key)
        {
            return App.Current.Resources[key] as string;
        }
    }
}
