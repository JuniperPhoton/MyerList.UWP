using JP.Utils.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace MyerList.UC
{
    public sealed partial class TitleBarUC : UserControl
    {
        public RoutedEventHandler BackHandler;
        
        public TitleBarUC()
        {
            this.InitializeComponent();
            if (DeviceHelper.IsDesktop)
                Window.Current.SetTitleBar(backGrdRect);
        }

        public void SetForegroundColor(Color color)
        {
            if(DeviceHelper.IsDesktop)
                TitleTB.Foreground=BackSymbol.Foreground = new SolidColorBrush(color);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            BackHandler?.Invoke(sender, e);
        }
    }
}
