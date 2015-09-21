using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace AnimatedTextBlock
{
    public class AnimatedTextBlock: ContentControl
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
            "TextContent", typeof(string), typeof(AnimatedTextBlock), new PropertyMetadata("", async(sender, e) =>
            {
                var tb = sender as AnimatedTextBlock;
                await tb.Animate(e);
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
           
            tcs.SetResult(0);
        }

        public void SetText1(string text)
        {
            this.textblock1.Text = text;
        }

        public void SetText2(string text)
        {
            this.textblock2.Text = text;
        }

        public async Task Animate(DependencyPropertyChangedEventArgs e)
        {
            await tcs.Task;
            SetText1(e.OldValue as string);
            SetText2(e.NewValue as string);
            ChangeStory.Completed += (senderc, ec) =>
            {
                ResetTransform(e.NewValue as string);
            };
            ChangeStory.Begin();
        }

        public void ResetTransform(string newStr)
        {
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
