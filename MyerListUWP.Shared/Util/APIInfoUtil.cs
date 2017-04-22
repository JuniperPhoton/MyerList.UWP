using System;
using Windows.Foundation.Metadata;

namespace MyerList.Util
{
    public class APIInfoUtil
    {
        public static bool HasHardwareButton
        {
            get
            {
                return CheckHardwareButton();
            }
        }

        public static bool HasStatusBar
        {
            get
            {
                return CheckStatusBar();
            }
        }

        private static bool CheckHardwareButton()
        {
            try
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool CheckStatusBar()
        {
            try
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
