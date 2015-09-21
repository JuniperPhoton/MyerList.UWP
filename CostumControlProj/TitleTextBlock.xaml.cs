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


namespace CCP
{
    public sealed partial class TitleTextBlock : UserControl
    {

        public string TextContent
        {
            get
            {
               return GetValue(TextContentProperty) as string;
            }
            set
            {
                SetValue(TextContentProperty, value);
            }
        }

        public static DependencyProperty TextContentProperty = DependencyProperty.Register(
            "TextContent", typeof(string), typeof(TitleTextBlock), new PropertyMetadata("", (sender, e) =>
                {
                    var tb = sender as TitleTextBlock;
                    tb.Text1.Text = e.OldValue as string;
                    tb.Text2.Text = e.NewValue as string;
                    tb.ChangeStory.Completed += (senderc, ec) =>
                      {
                          tb.Text1.Text = e.NewValue as string;
                          (tb.Text1.RenderTransform as CompositeTransform).TranslateX = 0;
                          tb.Text1.Opacity = 1;
                          tb.Text1.Visibility = Visibility.Visible;
                          (tb.Text2.RenderTransform as CompositeTransform).TranslateX = 100;
                          tb.Text2.Opacity = 0;
                          tb.Text2.Visibility = Visibility.Collapsed;
                      };
                    tb.ChangeStory.Begin();
                }));

        public TitleTextBlock()
        {
            this.InitializeComponent();
        }
    }
}
