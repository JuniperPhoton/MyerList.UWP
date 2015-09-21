using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyerList.Converter
{
    public class IsInReorderConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
#if WINDOWS_PHONE_APP
            ListViewReorderMode mode = (ListViewReorderMode)value;
            switch(mode)
            {
                case ListViewReorderMode.Enabled: return true;
                case ListViewReorderMode.Disabled: return false;
                
            }
#endif
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
