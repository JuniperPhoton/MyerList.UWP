using MyerList.ViewModel;
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
    public sealed partial class AddingPane : UserControl
    {
        public MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }
        public AddingPane()
        {
            this.InitializeComponent();
        }

        public void SetFocus()
        {
            AddContentBox.Focus(FocusState.Programmatic);
        }
    }
}
