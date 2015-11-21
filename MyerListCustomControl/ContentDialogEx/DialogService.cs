using JP.Utils.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyerListCustomControl
{
    public enum DialogKind
    {
        PlainText,
        InputContent
    }
    public class DialogService:ContentControl
    {
        #region DependencyProperty

        public string TitleText
        {
            get { return (string)GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public static DependencyProperty TitleTextProperty = DependencyProperty.Register("TitleText",
            typeof(string), typeof(DialogService), new PropertyMetadata("Title"));


        public string ContentText
        {
            get { return (string)GetValue(ContentTextProperty); }
            set { SetValue(ContentTextProperty, value); }
        }

        public static DependencyProperty ContentTextProperty = DependencyProperty.Register("ContentText",
            typeof(string), typeof(DialogService), new PropertyMetadata("Content"));

        public SolidColorBrush DialogBackground
        {
            get { return (SolidColorBrush)GetValue(DialogBackgroundProperty); }
            set { SetValue(DialogBackgroundProperty, value); }
        }
        public static DependencyProperty DialogBackgroundProperty = DependencyProperty.Register("DialogBackground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public SolidColorBrush DialogForeground
        {
            get { return (SolidColorBrush)GetValue(DialogForegroundProperty); }
            set { SetValue(DialogForegroundProperty, value); }
        }

        public static DependencyProperty DialogForegroundProperty = DependencyProperty.Register("DialogForeground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public SolidColorBrush LeftButtonBackground
        {
            get { return (SolidColorBrush)GetValue(LeftButtonBackgroundProperty); }
            set { SetValue(LeftButtonBackgroundProperty, value); }
        }

        public static DependencyProperty LeftButtonBackgroundProperty = DependencyProperty.Register("LeftButtonBackground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(null));

        public SolidColorBrush RightButtonBackground
        {
            get { return (SolidColorBrush)GetValue(RightButtonBackgroundProperty); }
            set { SetValue(RightButtonBackgroundProperty, value); }
        }

        public static DependencyProperty RightButtonBackgroundProperty = DependencyProperty.Register("RightButtonBackground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(null));

        public SolidColorBrush LeftButtonForeground
        {
            get { return (SolidColorBrush)GetValue(LeftButtonForegroundProperty); }
            set { SetValue(LeftButtonForegroundProperty, value); }
        }

        public static DependencyProperty LeftButtonForegroundProperty = DependencyProperty.Register("LeftButtonForeground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(null));

        public SolidColorBrush RightButtonForeground
        {
            get { return (SolidColorBrush)GetValue(RightButtonForegroundProperty); }
            set { SetValue(RightButtonForegroundProperty, value); }
        }

        public static DependencyProperty RightButtonForegroundProperty = DependencyProperty.Register("RightButtonForeground",
            typeof(SolidColorBrush), typeof(DialogService), new PropertyMetadata(null));

        public string LeftButtonContent
        {
            get { return (string)GetValue(LeftButtonContentProperty); }
            set { SetValue(LeftButtonContentProperty, value); }
        }

        public static DependencyProperty LeftButtonContentProperty = DependencyProperty.Register("LeftButtonContent",
            typeof(string), typeof(DialogService), new PropertyMetadata("OK"));

        public string RightButtonContent
        {
            get { return (string)GetValue(RightButtonContentProperty); }
            set { SetValue(RightButtonContentProperty, value); }
        }

        public static DependencyProperty RightButtonContentProperty = DependencyProperty.Register("RightButtonContent",
            typeof(string), typeof(DialogService), new PropertyMetadata("Cancel"));

        public string InputBoxHintText
        {
            get { return (string)GetValue(InputBoxHintTextProperty); }
            set { SetValue(InputBoxHintTextProperty, value); }
        }

        public static readonly DependencyProperty InputBoxHintTextProperty =
            DependencyProperty.Register("InputBoxHintText", typeof(string), typeof(DialogService), new PropertyMetadata("Input something."));



        public bool SelectAllWhenFocusInputBox
        {
            get { return (bool)GetValue(SelectAllWhenFocusInputBoxProperty); }
            set { SetValue(SelectAllWhenFocusInputBoxProperty, value); }
        }

        public static readonly DependencyProperty SelectAllWhenFocusInputBoxProperty =
            DependencyProperty.Register("SelectAllWhenFocusInputBox", typeof(bool), typeof(DialogService), new PropertyMetadata(true));



        public string InputBoxText
        {
            get { return (string)GetValue(InputBoxTextProperty); }
            set { SetValue(InputBoxTextProperty, value); }
        }

        public static readonly DependencyProperty InputBoxTextProperty =
            DependencyProperty.Register("InputBoxText", typeof(string), typeof(DialogService), new PropertyMetadata("",
                async(sender,e)=>
                {
                    var control = sender as DialogService;
                    await control.UpdateInputBoxContent();
                }));

        private bool _isOpen { get; set; }

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

        private CommandBar CurrentAppBar
        {
            get
            {
                return CurrentPage.BottomAppBar as CommandBar;
            }
        }

        private string _title;
        private string _content;
        private TextBlock _titleTB;
        private TextBlock _contentTB;
        private Button _leftBtn;
        private Button _rightBtn;
        private TextBlock _leftBtnTB;
        private TextBlock _rightBtnTB;
        private Grid _rootGrid;
        private Border _maskBorder;
        private Storyboard _inStory;
        private Storyboard _outStory;
        private TextBox _inputBox;

        //Use popup to show the control
        private Popup _currentPopup;

        public event Action<string> OnLeftBtnClick;
        public event Action OnRightBtnClick;

        //Provide the method to solve getting Storyboard before OnApplyTemplate() execute problem.
        private TaskCompletionSource<int> _tcs;

        private DialogKind _kind = DialogKind.PlainText;

        public DialogService()
        {
            DefaultStyleKey = (typeof(DialogService));

            if (!DesignMode.DesignModeEnabled)
            {
                _tcs = new TaskCompletionSource<int>();

                if (_currentPopup == null)
                {
                    _currentPopup = new Popup();
                    _currentPopup.VerticalAlignment = VerticalAlignment.Stretch;
                    this.Height = (Window.Current.Content as Frame).Height;
                    this.Width = (Window.Current.Content as Frame).Width;
                    _currentPopup.Child = this;
                    _currentPopup.IsOpen = true;
                }
            }

            CurrentPage.SizeChanged -= Page_SizeChanged;
            CurrentPage.SizeChanged += Page_SizeChanged;
        }

        public DialogService(DialogKind kind, string title, string content) :this()
        {
            this._kind = kind;
            if (!DesignMode.DesignModeEnabled)
            {
                _title = title;
                _content = content;
            }
        }

        public DialogService(string title, string content):this()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                _title = title;
                _content = content;
            }
        }



        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _rootGrid.Width = this.Width = e.NewSize.Width;
            _rootGrid.Height = this.Height = e.NewSize.Height;
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
            _rootGrid = GetTemplateChild("RootGrid") as Grid;
            _titleTB = GetTemplateChild("TitleTB") as TextBlock;
            _contentTB = GetTemplateChild("ContentTB") as TextBlock;
            _leftBtn = GetTemplateChild("LeftBtn") as Button;
            _rightBtn = GetTemplateChild("RightBtn") as Button;
            _leftBtnTB = GetTemplateChild("LeftBtnTB") as TextBlock;
            _rightBtnTB = GetTemplateChild("RightBtnTB") as TextBlock;
            _maskBorder = GetTemplateChild("MaskBorder") as Border;
            _inputBox = GetTemplateChild("InputBox") as TextBox;

            if (_kind == DialogKind.InputContent)
            {
                _inputBox.Visibility = Visibility.Visible;
            }
            else _inputBox.Visibility = Visibility.Collapsed;

            _maskBorder.Tapped += ((sendert, et) =>
            {
                if (!_isOpen)
                {
                    return;
                }
                Hide();
            });

            _inStory = _rootGrid.Resources["InStory"] as Storyboard;
            _outStory = _rootGrid.Resources["OutStory"] as Storyboard;
            _outStory.Completed += OutStory_Completed;

            _tcs.TrySetResult(1);

            _rootGrid.Width = Window.Current.Bounds.Width;
            _rootGrid.Height = Window.Current.Bounds.Height;

            _leftBtn.Click += LeftBtn_Click;
            _rightBtn.Click += RightBtn_Click;

            TitleText = _title;
            ContentText = _content;
        }

        public async Task UpdateInputBoxContent()
        {
            await _tcs.Task;
            _inputBox.Text = InputBoxText;
            if (SelectAllWhenFocusInputBox)
            {
                _inputBox.SelectAll();
            }
        }

        private void OutStory_Completed(object sender, object e)
        {
            _currentPopup.IsOpen = false;
        }

        private void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            OnRightBtnClick?.Invoke();
            this.Hide();
        }

        private void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            OnLeftBtnClick?.Invoke(_inputBox.Text);
            this.Hide();
        }

        public async Task ShowAsync()
        {
            if (CurrentAppBar != null)
            {
                CurrentAppBar.IsSticky = false;
                CurrentAppBar.Visibility = Visibility.Collapsed;
            }

            if (_isOpen)
            {
                return;
            }

            await _tcs.Task;

            _isOpen = true;
            _maskBorder.Visibility = Visibility.Visible;

            _inStory.Begin();

            if (ApiInformationHelper.HasHardwareButton)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += BottomDialog_BackRequested;
        }

        private void BottomDialog_BackRequested(object sender, BackRequestedEventArgs e)
        {
            HandleBack();
            e.Handled = true;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            HandleBack();
            e.Handled = true;
        }

        private void HandleBack()
        {
            if (_isOpen)
            {
                Hide();
            }
        }

        public void Hide()
        {
            if (CurrentAppBar != null)
            {
                CurrentAppBar.IsSticky = true;
                CurrentAppBar.Visibility = Visibility.Visible;
            }

            _isOpen = false;
            _outStory.Begin();
            _maskBorder.Visibility = Visibility.Collapsed;

            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= BottomDialog_BackRequested;
        }
    }
}

