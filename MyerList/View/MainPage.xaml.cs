using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Helper;
using Lousy.Mon;
using MyerList.Base;
using MyerList.Helper;
using MyerList.UC;
using MyerList.ViewModel;
using MyerListCustomControl;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using System;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MyerListUWP.View
{
    public sealed partial class MainPage : BindablePage
    {
        //数据源
        public MainViewModel MainVM
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        //当前抽屉是否出现了
        private bool _isDrawerSlided = false;

        //当前的手指位置，用于手势
        private double _pointOriX = 0;

        //添加/修改的面板是否出现
        public bool IsAddingPaneOpen
        {
            get { return (bool)GetValue(IsAddingPaneOpenProperty); }
            set { SetValue(IsAddingPaneOpenProperty, value); }
        }

        public static DependencyProperty IsAddingPaneOpenProperty =
            DependencyProperty.Register("IsAddingPaneOpen", typeof(bool), typeof(MainPage), new PropertyMetadata(false,
               IsAddingPaneOpenPropertyChanged));

        public static void IsAddingPaneOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var page = d as MainPage;
            if (args.NewValue == args.OldValue) return;
            if ((bool)args.NewValue) page.EnterAddMode();
            else page.LeaveAddmode();
        }

        private Compositor _compositor;
        private Visual _drawerVisual;
        private Visual _drawerMaskVisual;

        public MainPage()
        {
            this.InitializeComponent();

            InitFeature();
            InitBinding();
            InitComposition();

            CoreWindow.GetForCurrentThread().SizeChanged += MainPage_SizeChanged;

            RegisterMessenger();

            this.Loaded += MainPage_Loaded;

            MainVM.OnCateColorChanged += MainVM_OnCategoryChanged;

            InitialLayout();
        }

        private void InitComposition()
        {
            _drawerVisual = ElementCompositionPreview.GetElementVisual(Drawer);
            _drawerMaskVisual = ElementCompositionPreview.GetElementVisual(MaskBorder);
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _drawerMaskVisual.Opacity = 0;
            _drawerVisual.Offset = new Vector3(-250, 0f, 0f);

            MaskBorder.Visibility = Visibility.Collapsed;
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowPaneOpen"),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, IsAddingPaneOpenProperty, b);
        }

        private void InitFeature()
        {
            if (LocalSettingHelper.HasValue(ResourcesHelper.GetDicString("FeatureToken")))
            {
                if (LocalSettingHelper.GetValue(ResourcesHelper.GetDicString("FeatureToken")) == "true")
                {
                    FeatureGrid.Visibility = Visibility.Collapsed;
                }
                else FeatureGrid.Visibility = Visibility.Visible;
            }
            else FeatureGrid.Visibility = Visibility.Visible;
        }

        private void MainVM_OnCategoryChanged()
        {
            if (Window.Current.Bounds.Width >= 720)
            {
                HamburgerBtn.ForegroundBrush = MainVM.CateColor;
                TitleTB.Foreground = MainVM.CateColor;
                ProgressRing.Foreground = MainVM.CateColor;
                TitleBarHelper.SetUpForeBlackTitleBar();
            }
            else
            {
                var solidColor = HeaderContentRootGrid.Background as SolidColorBrush;
                if (solidColor == null) return;
                if (solidColor.Color != MainVM.CateColor.Color)
                {
                    ChangeColorAnim.To = MainVM.CateColor.Color;
                    ChangeColorAnim.From = Colors.White;
                    ChangeColorStory.Begin();
                }
                HamburgerBtn.ForegroundBrush = new SolidColorBrush(Colors.White);
                TitleTB.Foreground = new SolidColorBrush(Colors.White);
                ProgressRing.Foreground = new SolidColorBrush(Colors.White);
                TitleBarHelper.SetUpForeWhiteTitleBar();
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            AddingPaneFirstOffset.Value = -this.ActualWidth;
            AddingPaneLastOffset.Value = -this.ActualWidth;
            AddingPanelTransform.TranslateX = -this.ActualWidth;
        }

        private void InitialLayout()
        {
            if (Window.Current.Bounds.Width >= 720)
            {
                HeaderContentRootGrid.Background = new SolidColorBrush(Colors.White);
                HamburgerBtn.ForegroundBrush = App.Current.Resources["MyerListBlue"] as SolidColorBrush;
                TitleTB.Foreground = App.Current.Resources["MyerListBlue"] as SolidColorBrush;
                ProgressRing.Foreground = App.Current.Resources["MyerListBlue"] as SolidColorBrush;
            }
            else
            {
                HeaderContentRootGrid.Background = App.Current.Resources["MyerListBlue"] as SolidColorBrush;
                HamburgerBtn.ForegroundBrush = new SolidColorBrush(Colors.White);
                TitleTB.Foreground = new SolidColorBrush(Colors.White);
                ProgressRing.Foreground = new SolidColorBrush(Colors.White);
            }
            MainPage_SizeChanged(null, null);
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.CloseHam, msg =>
            {
                if (_isDrawerSlided && CoreWindow.GetForCurrentThread().Bounds.Width < 720)
                {
                    ToggleDrawerAnimation(false);
                    ToggleDrawerMaskAnimation(false);
                    HamburgerBtn.PlayHamOutStory();
                    _isDrawerSlided = false;
                }
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.ChangeCommandBarToDelete, msg =>
            {
                SwitchCommandBarToDelete.Begin();
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.ChangeCommandBarToDefault, msg =>
            {
                SwitchCommandBarToDefault.Begin();
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.GoToSort, async act =>
            {
                DisplayedListView.CanDragItems = true;
                DisplayedListView.CanReorderItems = true;
                DisplayedListView.AllowDrop = true;
                DisplayedListView.IsItemClickEnabled = false;
                await ToastService.SendToastAsync(ResourcesHelper.GetResString("ReorderHint"));
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.LeaveSort, act =>
            {
                DisplayedListView.CanDragItems = false;
                DisplayedListView.CanReorderItems = false;
                DisplayedListView.AllowDrop = false;
                DisplayedListView.IsItemClickEnabled = true;
            });
        }

        #region SizeChanged
        private void MainPage_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
            var left = 0d;
            var right = 0d;

            if (Window.Current.Bounds.Width >= 720)
            {
                left = 270;
                right = (Window.Current.Bounds.Width - 250) / 5d;
                _isDrawerSlided = true;

            }
            else
            {
                _isDrawerSlided = false;

            }

            UpdateColorWhenSizeChanged();

            if (ListContentGrid.Margin.Left != left)
            {
                ListContentGrid.Margin = new Thickness(left, 0, right, 0);
                HeaderContentGrid.Margin = new Thickness(left, 0, right, 20);
            }

            AddingPaneFirstOffset.Value = -this.ActualWidth;
            AddingPaneLastOffset.Value = -this.ActualWidth;
            AddingPanelTransform.TranslateX = -this.ActualWidth;

            if (!_isDrawerSlided)
            {
                _drawerVisual.Offset = new Vector3(-250f, 0f, 0f);
            }
        }

        bool _isToggleAnim1 = false;
        bool _isToggleAnim2 = false;

        private void UpdateColorWhenSizeChanged()
        {
            if (Window.Current.Bounds.Width >= 720)
            {
                if (!_isToggleAnim1)
                {
                    ChangeColorAnim.To = Colors.White;
                    ChangeColorAnim.From = (HeaderContentRootGrid.Background as SolidColorBrush).Color;
                    ChangeColorStory.Begin();
                    _isToggleAnim1 = true;
                    _isToggleAnim2 = false;
                    ToggleDrawerAnimation(true);
                }
                HamburgerBtn.ForegroundBrush = MainVM.CateColor;
                TitleTB.Foreground = MainVM.CateColor;
                ProgressRing.Foreground = MainVM.CateColor;
                TitleBarHelper.SetUpForeBlackTitleBar();
            }
            else
            {
                if (!_isToggleAnim2)
                {
                    ChangeColorAnim.To = MainVM.CateColor.Color;
                    ChangeColorAnim.From = Colors.White;
                    ChangeColorStory.Begin();
                    _isToggleAnim2 = true;
                    _isToggleAnim1 = false;
                    ToggleDrawerAnimation(false);
                }
                HamburgerBtn.ForegroundBrush = new SolidColorBrush(Colors.White);
                TitleTB.Foreground = new SolidColorBrush(Colors.White);
                ProgressRing.Foreground = new SolidColorBrush(Colors.White);
                TitleBarHelper.SetUpForeWhiteTitleBar();
            }
        }

        #endregion

        #region CommandBar
        private double origianlTranslateX = 0;
        private void EnterAddMode()
        {
            var transfrom = Drawer.RenderTransform as CompositeTransform;
            origianlTranslateX = transfrom.TranslateX;

            Oli.MoveXOf(Drawer).From(origianlTranslateX).To(transfrom.TranslateX - 100).
                With(new CubicEase() { EasingMode = EasingMode.EaseOut }).For(0.5, OrSo.Seconds).Now();

            AddStory.Begin();
            AddingPanel.SetFocus();

            TitleBarHelper.SetUpForeWhiteTitleBar();
        }

        private void LeaveAddmode()
        {
            RemoveStory.Begin();

            var transfrom = Drawer.RenderTransform as CompositeTransform;
            Oli.MoveXOf(Drawer).To(origianlTranslateX).From(transfrom.TranslateX - 100).
                With(new CubicEase() { EasingMode = EasingMode.EaseOut }).For(0.5, OrSo.Seconds).Now();

            if (Window.Current.Bounds.Width >= 720)
            {
                TitleBarHelper.SetUpForeBlackTitleBar();
            }
        }
        #endregion

        #region Drawer
        #region Drawer animation trigger
        private void ToggleDrawerAnimation(bool show)
        {
            var offsetAnim = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnim.InsertKeyFrame(1f, show ? 0f : -250);
            offsetAnim.Duration = TimeSpan.FromMilliseconds(300);

            _drawerVisual.StartAnimation("Offset.X", offsetAnim);
        }

        private void ToggleDrawerMaskAnimation(bool show)
        {
            if (show) MaskBorder.Visibility = Visibility.Visible;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 1f : 0f, _compositor.CreateLinearEasingFunction());
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(300);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _drawerMaskVisual.StartAnimation("Opacity", fadeAnimation);
            batch.Completed += (sender, e) =>
            {
                if (!show) MaskBorder.Visibility = Visibility.Collapsed;
            };
            batch.End();
        }
        #endregion

        private void HamClick(object sender, RoutedEventArgs e)
        {
            _isDrawerSlided = true;
            MaskBorder.Visibility = Visibility.Visible;
            ToggleDrawerAnimation(true);
            ToggleDrawerMaskAnimation(true);
        }

        private void MaskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isDrawerSlided)
            {
                _isDrawerSlided = false;
                ToggleDrawerAnimation(false);
                ToggleDrawerMaskAnimation(false);
            }
        }
        #endregion

        #region Override

        protected override void SetNavigationBackBtn()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        protected override void RegisterHandleBackLogic()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += NewMainPage_BackRequested;
            if (APIInfoHelper.HasHardwareButton)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        protected override void UnRegisterHandleBackLogic()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= NewMainPage_BackRequested;
            if (APIInfoHelper.HasHardwareButton)
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (HandleBackLogic()) e.Handled = true;
            else e.Handled = false;
        }

        private void NewMainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (HandleBackLogic()) e.Handled = true;
            else e.Handled = false;
        }

        private bool _readyToExit = false;
        DispatcherTimer _timer = new DispatcherTimer();

        /// <summary>
        /// 处理返回逻辑
        /// </summary>
        /// <returns>是否已经被处理了</returns>
        private bool HandleBackLogic()
        {
            if (FeatureGrid.Visibility == Visibility.Visible)
            {
                FeatureOkClick(null, null);
                return true;
            }
            if (MainVM.ShowPaneOpen)
            {
                MainVM.ShowPaneOpen = false;
                return true;
            }
            if (MainVM.SelectedCate != 0)
            {
                MainVM.SelectedCate = 0;
                return true;
            }
            if (_isDrawerSlided)
            {
                ToggleDrawerAnimation(false);
                ToggleDrawerMaskAnimation(false);
                _isDrawerSlided = false;
                return true;
            }
            return false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DeviceHelper.IsMobile)
            {
                StatusBarHelper.SetUpStatusBar();
            }
            Frame.BackStack.Clear();

            var titleBarUC = new EmptyTitleControl();
            (this.Content as Grid).Children.Add(titleBarUC);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (_isDrawerSlided && CoreWindow.GetForCurrentThread().Bounds.Width < 720)
            {
                ToggleDrawerAnimation(false);
                ToggleDrawerMaskAnimation(false);
                HamburgerBtn.PlayHamOutStory();
                _isDrawerSlided = false;
            }
        }

        #endregion

        #region Feature
        private void FeatureOkClick(object sender, RoutedEventArgs e)
        {
            var anim1 = Oli.Fade(FeatureGrid).From(1).To(0).For(0.2, OrSo.Seconds).Now();
            Oli.Run(() =>
            {
                FeatureGrid.Visibility = Visibility.Collapsed;
            }).After(anim1);
            LocalSettingHelper.AddValue(ResourcesHelper.GetDicString("FeatureToken"), "true");
        }
        #endregion

        #region Drawer manipulation
        private void TouchGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X >= 70)
            {
                _isDrawerSlided = true;
                ToggleDrawerAnimation(true);
                ToggleDrawerMaskAnimation(true);
            }
            else
            {
                _isDrawerSlided = false;
                ToggleDrawerAnimation(false);
                ToggleDrawerMaskAnimation(false);
            }
        }

        private void TouchGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_drawerMaskVisual.Opacity < 1)
            {
                MaskBorder.Visibility = Visibility.Visible;
                _drawerMaskVisual.Opacity += 0.02f;
            }
            var targetOffsetX = _drawerVisual.Offset.X + e.Delta.Translation.X;
            _drawerVisual.Offset = new Vector3((float)(targetOffsetX > 1 ? 1 : targetOffsetX), 0f, 0f);
        }
        #endregion
    }
}
