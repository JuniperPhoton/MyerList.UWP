using MyerListUWP.Common.Composition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerList.UC
{
    public sealed partial class LoadingControl : UserControl
    {
        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(LoadingControl),
                new PropertyMetadata(false, OnIsShownPropertyChanged));

        private static void OnIsShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LoadingControl;
            control.ToggleAnimation((bool)e.NewValue);
        }

        private Compositor _compositor;
        private Visual _rootVisual;

        public LoadingControl()
        {
            this.InitializeComponent();
            this.InitComposition();
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _rootVisual = RootGrid.GetVisual();
            _rootVisual.Opacity = 0;
            RootGrid.Visibility = Visibility.Collapsed;
        }

        private void ToggleAnimation(bool show)
        {
            RootGrid.Visibility = Visibility.Visible;

            var animation = _compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1f, show ? 1f : 0f);
            animation.Duration = TimeSpan.FromMilliseconds(500);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _rootVisual.StartAnimation("Opacity", animation);
            batch.Completed += (sender, e) =>
              {
                  if (!show) RootGrid.Visibility = Visibility.Collapsed;
              };
            batch.End();
        }
    }
}
