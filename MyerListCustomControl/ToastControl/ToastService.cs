using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace MyerListCustomControl
{
    public class ToastService : Control
    {
        #region DependencyProperty

        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        public static DependencyProperty ContentTextProperty = DependencyProperty.Register("ContentText",
            typeof(string), typeof(ToastService), new PropertyMetadata("Content"));

        public TimeSpan HideTimeSpan
        {
            get { return (TimeSpan)GetValue(HideTimeSpanProperty); }
            set { SetValue(HideTimeSpanProperty, value); }
        }

        public static readonly DependencyProperty HideTimeSpanProperty =
            DependencyProperty.Register("HideTimeSpan", typeof(TimeSpan), typeof(ToastService), new PropertyMetadata(TimeSpan.FromSeconds(2.0)));

        #endregion

        private Page _currentPage;
        public Page CurrentPage
        {
            get
            {
                if (_currentPage != null) return _currentPage;
                else return ((Window.Current.Content as Frame).Content) as Page;
            }
            set
            {
                _currentPage = value;
            }
        }

        private string _tempText;

        private Grid _rootGrid;
        private TextBlock _contentTB;
        private Storyboard _playStory;

        //Use popup to show the control
        private Popup _currentPopup;

        //Provide the method to solve getting Storyboard before OnApplyTemplate() execute problem.
        private TaskCompletionSource<int> _tcs;

        private ToastService()
        {
            DefaultStyleKey = (typeof(ToastService));

            if (!DesignMode.DesignModeEnabled)
            {
                _tcs = new TaskCompletionSource<int>();

                if (_currentPopup == null)
                {
                    _currentPopup = new Popup();
                    _currentPopup.VerticalAlignment = VerticalAlignment.Stretch;

                    this.Width = Window.Current.Bounds.Width;
                    this.Height = Window.Current.Bounds.Height;

                    _currentPopup.Child = this;
                    _currentPopup.IsOpen = true;
                }
            }

            CurrentPage.SizeChanged -= Page_SizeChanged;
            CurrentPage.SizeChanged += Page_SizeChanged;
        }

        private ToastService(string text) : this()
        {
            _tempText = text;
        }

        private ToastService(string text, TimeSpan time) : this()
        {
            _tempText = text;
            HideTimeSpan = time;
        }

        public static void SendToast(string text)
        {
            ToastService ts = new ToastService(text);
            var task = ts.ShowAsync();
        }

        [Obsolete("Please user SendToast(string) instead")]
        public static async Task SendToastAsync(string text)
        {
            ToastService ts = new ToastService(text);
            await ts.ShowAsync();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var task = UpdateSize();
        }

        private async Task UpdateSize()
        {
            await _tcs.Task;
            _rootGrid.Width = this.Width = Window.Current.Bounds.Width;
            _rootGrid.Height = this.Height = Window.Current.Bounds.Height;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!DesignMode.DesignModeEnabled)
            {
                InitialPane();
            }
        }

        private void InitialPane()
        {
            _contentTB = GetTemplateChild("ContentTB") as TextBlock;
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _playStory = _rootGrid.Resources["PlayStory"] as Storyboard;
            _contentTB.Text = _tempText;
            _tcs.SetResult(0);
        }

        public async Task ShowAsync()
        {
            await _tcs.Task;
            await UpdateSize();
            _playStory.Begin();
            await Task.Delay(HideTimeSpan);
            _currentPopup.IsOpen = false;
        }
    }
}
