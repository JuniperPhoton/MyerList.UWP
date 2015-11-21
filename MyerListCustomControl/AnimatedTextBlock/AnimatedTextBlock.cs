using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyerListCustomControl
{
    public class AnimatedTextBlock : ContentControl
    {
        private TextBlock textblock1;
        private TextBlock textblock2;
        private Grid rootGrid;

        public Storyboard ChangeStory;

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
            "TextContent", typeof(string), typeof(AnimatedTextBlock), new PropertyMetadata("", async (sender, e) =>
            {
                var tb = sender as AnimatedTextBlock;
                if (e.NewValue != e.OldValue && !string.IsNullOrEmpty(e.OldValue as string))
                {
                    await tb.Animate(e);
                }
                else if (e.NewValue != e.OldValue)
                {
                    await tb.ResetTransformAsync(e.NewValue as string);
                }
            }));

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
            textblock1 = GetTemplateChild("Text1") as TextBlock;
            textblock2 = GetTemplateChild("Text2") as TextBlock;
            rootGrid = GetTemplateChild("RootGrid") as Grid;
            ChangeStory = rootGrid.Resources["ChangeStory"] as Storyboard;
            rootGrid.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), new Size(400, 50)) };
            tcs.SetResult(0);
        }

        public async Task Animate(DependencyPropertyChangedEventArgs e)
        {
            await tcs.Task;
            textblock1.Text = e.OldValue as string;
            textblock2.Text = e.NewValue as string;

            ChangeStory.Completed += async (senderc, ec) =>
            {
                await ResetTransformAsync(e.NewValue as string);
            };
            ChangeStory.Begin();
        }

        public async Task ResetTransformAsync(string newStr)
        {
            await tcs.Task;
            textblock1.Text = newStr;

            (textblock1.RenderTransform as CompositeTransform).TranslateX = 0;
            textblock1.Opacity = 1;
            textblock1.Visibility = Visibility.Visible;
            (textblock2.RenderTransform as CompositeTransform).TranslateX = 100;
            textblock2.Opacity = 0;
            textblock2.Visibility = Visibility.Collapsed;
        }
    }
}
