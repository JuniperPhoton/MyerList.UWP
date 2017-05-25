using MyerListUWP.Common.Composition;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyerListCustomControl
{
    public class AnimatedTextBlock : ContentControl
    {
        private TextBlock textblock1;
        private TextBlock textblock2;
        private Grid rootGrid;

        private Compositor _compositor;
        private Visual _tb1Visual;
        private Visual _tb2Visual;

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
            "TextContent", typeof(string), typeof(AnimatedTextBlock), 
            new PropertyMetadata("", OnTextContentPropertyChanged));

        private static async void OnTextContentPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs e)
        {
            var tb = d as AnimatedTextBlock;
            if (e.NewValue != e.OldValue && !string.IsNullOrEmpty(e.OldValue as string))
            {
                await tb.Animate((string)e.NewValue,(string)e.OldValue);
            }
            else if (e.NewValue != e.OldValue)
            {
                await tb.ResetOffsetAsync(e.NewValue as string);
            }
        }

        public TaskCompletionSource<int> tcs;

        public AnimatedTextBlock()
        {
            DefaultStyleKey = (typeof(AnimatedTextBlock));

            tcs = new TaskCompletionSource<int>();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignMode.DesignModeEnabled)
            {
                Initial();
            }
        }

        private void Initial()
        {
            textblock1 = GetTemplateChild("TB1") as TextBlock;
            textblock2 = GetTemplateChild("TB2") as TextBlock;
            rootGrid = GetTemplateChild("RootGrid") as Grid;

            _compositor = ElementCompositionPreview.GetElementVisual(rootGrid).Compositor;
            _tb1Visual = textblock1.GetVisual();
            _tb2Visual = textblock2.GetVisual();
            _tb2Visual.Opacity = 0f;
            _tb2Visual.Offset = new Vector3(50f, 0f, 0f);

            rootGrid.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), new Size(1400, 50)) };
            tcs.SetResult(0);
        }

        public async Task Animate(string newValue,string oldValue)
        {
            await tcs.Task;
            textblock1.Text = oldValue;
            textblock2.Text = newValue;

            var offsetAnimation1 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation1.InsertKeyFrame(1f, -50f);
            offsetAnimation1.Duration = TimeSpan.FromMilliseconds(400);

            var offsetAnimation2 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation2.InsertKeyFrame(1f, 0f);
            offsetAnimation2.Duration= TimeSpan.FromMilliseconds(400);

            var fadeAnimation1 = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation1.InsertKeyFrame(1f, 0f);
            fadeAnimation1.Duration = TimeSpan.FromMilliseconds(400);

            var fadeAnimation2 = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation2.InsertKeyFrame(1f, 1f);
            fadeAnimation2.Duration = TimeSpan.FromMilliseconds(400);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _tb1Visual.StartAnimation(_tb1Visual.GetTranslationXPropertyName(), offsetAnimation1);
            _tb2Visual.StartAnimation(_tb2Visual.GetTranslationXPropertyName(), offsetAnimation2);
            _tb1Visual.StartAnimation("Opacity", fadeAnimation1);
            _tb2Visual.StartAnimation("Opacity", fadeAnimation2);
            batch.Completed += async(sender, ex) =>
              {
                  await ResetOffsetAsync(newValue);
              };
            batch.End();
        }

        public async Task ResetOffsetAsync(string newValue)
        {
            await tcs.Task;
            textblock1.Text = newValue;

            _tb1Visual.SetTranslation(new Vector3(0f, 0f, 0f));
            _tb1Visual.Opacity = 1f;

            _tb2Visual.SetTranslation(new Vector3(50f, 0f, 0f));
            _tb2Visual.Opacity = 0;
        }
    }
}
