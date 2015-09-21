using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerList.UC
{
    public sealed partial class ToastUC : UserControl
    {
        public ToastUC()
        {
            this.InitializeComponent();
        }


        public void ShowMessage(string msg)
        {
            this.Visibility = Visibility.Visible;
            root.Visibility = Visibility.Visible;
            this.messageTB.Text = msg;
            StartStory.Completed += ((sc, ec) =>
            {
                root.Visibility = Visibility.Collapsed;
                this.Visibility = Visibility.Collapsed;
            });
            StartStory.Begin();
        }
    }
}
