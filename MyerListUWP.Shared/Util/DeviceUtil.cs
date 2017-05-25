using Windows.ApplicationModel.Resources.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace MyerListUWP.Shared.Util
{
    public static class DeviceUtil
    {
        public static bool IsDesktop
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Desktop";
            }
        }

        public static bool IsMobile
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile";
            }
        }

        public static bool IsIoT
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "IoT";
            }
        }

        public static bool IsXbox
        {
            get
            {
                var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
                return qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Xbox";
            }
        }

        private static string[] GetDeviceOsVersion()
        {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            return new string[] { v1.ToString(), v2.ToString(), v3.ToString(), v4.ToString() };
        }

        public static string OSVersion
        {
            get
            {
                string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = (v & 0x000000000000FFFFL);
                return $"{v1}.{v2}.{v3}.{v4}";
            }
        }

        public static bool IsTH1OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "10240";
            }
        }

        public static bool IsTH2OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "10586";
            }
        }

        public static bool IsRS1OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "14393";
            }
        }

        public static bool IsRS2OS
        {
            get
            {
                var versions = GetDeviceOsVersion();
                return versions[2] == "15063";
            }
        }

        public static string DeviceModel
        {
            get
            {
                var eas = new EasClientDeviceInformation();
                return eas.SystemProductName;
            }
        }
    }
}
