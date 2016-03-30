using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerListComposition
{
    public sealed partial class MainPage : Page
    {
        private Visual _root;
        private Compositor _compositor;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _root = ElementCompositionPreview.GetElementVisual(rootGrid);
            _compositor = _root.Compositor;

            var visual1 = _compositor.CreateSpriteVisual();
            visual1.Size = new Vector2(400, 200);
            visual1.Offset = new Vector3(100, 100, 0);
            visual1.Brush = _compositor.CreateColorBrush(Colors.Black);

            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0f, 1f);
            animation.InsertKeyFrame(0.5f, 0.5f);
            animation.InsertKeyFrame(1f, 0f);
            animation.Duration = TimeSpan.FromSeconds(1);
            animation.IterationBehavior = AnimationIterationBehavior.Forever;

            ElementCompositionPreview.SetElementChildVisual(rootGrid, visual1);

            _root.StartAnimation("Opacity", animation);
        }
    }
}
