using JP.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace MyerList.Common
{
    public static class GlobalHelper
    {
        public static void SetupLang()
        {
            if (LocalSettingHelper.HasValue("AppLang") == false)
            {
                var lang = GlobalizationPreferences.Languages[0];
                if (lang.Contains("zh"))
                {
                    ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
                }
                else ApplicationLanguages.PrimaryLanguageOverride = "en-US";

                LocalSettingHelper.AddValue("AppLang", ApplicationLanguages.PrimaryLanguageOverride);
            }
            else ApplicationLanguages.PrimaryLanguageOverride = LocalSettingHelper.GetValue("AppLang");
        }
    }
}
