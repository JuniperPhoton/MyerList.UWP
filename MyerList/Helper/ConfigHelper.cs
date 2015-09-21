using System;
using System.Collections.Generic;
using System.Text;
using ChaoFunctionRT;
using JP.Utils.Data;

namespace MyerList.Helper
{
    public class ConfigHelper
    {
        public static void CheckConfig()
        {
            if (!LocalSettingHelper.HasValue("EnableTile"))
            {
                LocalSettingHelper.AddValue("EnableTile", "true");
            }

            if (!LocalSettingHelper.HasValue("EnableBackgroundTask"))
            {
                LocalSettingHelper.AddValue("EnableBackgroundTask", "true");
            }

            if (!LocalSettingHelper.HasValue("EnableGesture"))
            {
                LocalSettingHelper.AddValue("EnableGesture", "true");
            }

            if (!LocalSettingHelper.HasValue("ShowKeyboard"))
            {
                LocalSettingHelper.AddValue("ShowKeyboard", "true");
            }

            if (!LocalSettingHelper.HasValue("TransparentTile"))
            {
                LocalSettingHelper.AddValue("TransparentTile", "true");
            }

            if (!LocalSettingHelper.HasValue("AddMode"))
            {
                LocalSettingHelper.AddValue("AddMode", "1");
            }

            if(!LocalSettingHelper.HasValue("ThemeColor"))
            {
                LocalSettingHelper.AddValue("ThemeColor", "0");
            }
        }
    }
}
