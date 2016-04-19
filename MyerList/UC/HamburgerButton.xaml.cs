using MyerList.ViewModel;
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
    public sealed partial class HamburgerButton : UserControl
    {
        public MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public Brush ForegroundBrush
        {
            get { return (Brush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ForegroundBrushProperty =
            DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(HamburgerButton), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public event RoutedEventHandler ButtonClick;

        public HamburgerButton()
        {
            this.InitializeComponent();
            this.HamBtn.Click += ButtonClick;
        }

        public void PlayHamInStory()
        {
            //HamInStory.Begin();
        }

        public void PlayHamOutStory()
        {
            //HamOutStory.Begin();
        }

        private void HamClick(object sender,RoutedEventArgs e)
        {
            //PlayHamInStory();
            ButtonClick(sender, e);
        }
    }
}
