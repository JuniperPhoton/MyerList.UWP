using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerList.Model;
using MyerList.UC;
using MyerList.ViewModel;
using MyerListCustomControl;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.Foundation;
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
using Windows.UI.Xaml.Navigation;

namespace MyerListUWP.View
{
    public sealed partial class MainPage : BindablePage
    {
        private static double WIDTH_THRESHOLD => 650;

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
            page.ToggleAddingPanelAnimation((bool)args.NewValue);
        }

        private Compositor _compositor;
        private Visual _drawerVisual;
        private Visual _drawerMaskVisual;
        private Visual _addingPanelVisual;
        private Visual _defaultCommandBarVisual;
        private Visual _deleteComamndBarVisual;
        private Visual _contentRootGirdVisual;
        private Visual _headerVisual;
        private Visual _hamburgerVisual;

        public MainPage()
        {
            this.InitializeComponent();

            if(!DesignMode.DesignModeEnabled)
            {
                this.DataContext = new MainViewModel();

                //InitFeature();
                InitBinding();
                InitComposition();
                InitialLayout();

                this.SizeChanged += MainPage_SizeChanged;

                MainVM.OnCateColorChanged += MainVM_OnCategoryChanged;

                App.MainVM = this.MainVM;

                RegisterMessenger();
            }
        }

        private void InitComposition()
        {
            _drawerVisual = ElementCompositionPreview.GetElementVisual(Drawer);
            _drawerMaskVisual = ElementCompositionPreview.GetElementVisual(MaskBorder);
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _addingPanelVisual = ElementCompositionPreview.GetElementVisual(AddingPanel);
            _contentRootGirdVisual = ElementCompositionPreview.GetElementVisual(ContentRootGird);
            _defaultCommandBarVisual = ElementCompositionPreview.GetElementVisual(DefaultCommandBar);
            _deleteComamndBarVisual = ElementCompositionPreview.GetElementVisual(DeleteCommandBar);
            _headerVisual = ElementCompositionPreview.GetElementVisual(HeaderSP);
            _hamburgerVisual = ElementCompositionPreview.GetElementVisual(HamburgerBtn);

            _drawerMaskVisual.Opacity = 0;
            _drawerVisual.Offset = new Vector3(-250, 0f, 0f);
            _deleteComamndBarVisual.Offset = new Vector3(0f, 50f, 0f);

            _addingPanelVisual.Offset = new Vector3(-(float)this.ActualWidth, 0f, 0f);

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

        private void InitialLayout()
        {
            if (Window.Current.Bounds.Width >= WIDTH_THRESHOLD)
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
            //MainPage_SizeChanged(null, null);
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<GenericMessage<ObservableCollection<ToDo>>>(this, MessengerTokens.UpdateTile, async msg =>
            {
                await TileControl.UpdateTileAsync(msg.Content);
            });
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
            Messenger.Default.Register(this, MessengerTokens.ChangeCommandBarToDelete, (GenericMessage<string> msg) =>
            {
                this.SwitchToDeleteCommandBar();
            });
            Messenger.Default.Register(this, MessengerTokens.ChangeCommandBarToDefault, (GenericMessage<string> msg) =>
            {
                this.SwitchToDefaultCommandBar();
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
        bool _isToggleAnim1 = false;
        bool _isToggleAnim2 = false;

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsAddingPaneOpen)
            {
                _addingPanelVisual.Offset = new Vector3(-(float)this.ActualWidth, 0f, 0f);
            }

            if (e?.NewSize.Width > WIDTH_THRESHOLD && e.PreviousSize.Width<=WIDTH_THRESHOLD && !_isDrawerSlided)
            {
                SwitchToWideStory.Begin();
                ToggleDrawerAnimation(true);
                _isDrawerSlided = true;
                ChangeCommandBarColorToWhiteStory.Begin();
                ToggleHeaderAnimation(false);
            }
            else if (e?.NewSize.Width <= WIDTH_THRESHOLD && e?.PreviousSize.Width>WIDTH_THRESHOLD && _isDrawerSlided)
            {
                SwitchToNarrowStory.Begin();
                ToggleDrawerAnimation(false);
                ToggleDrawerMaskAnimation(false);
                _isDrawerSlided = false;
                ChangeCommandBarColorToGreyStory.Begin();
                ToggleHeaderAnimation(true);
            }
            UpdateColorWhenSizeChanged();
        }

        private void UpdateColorWhenSizeChanged()
        {
            if (Window.Current.Bounds.Width >= WIDTH_THRESHOLD)
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
        private void ToggleAddingPanelAnimation(bool show)
        {
            if (show)
            {
                ToggleAddingAnimation(true);
                TitleBarHelper.SetUpForeWhiteTitleBar();
                ToggleAnimationWithAddingPanel(false);
                AddingPanel.SetFocus();
            }
            else
            {
                ToggleAddingAnimation(false);
                ToggleAnimationWithAddingPanel(true);

                if (Window.Current.Bounds.Width >= WIDTH_THRESHOLD)
                {
                    TitleBarHelper.SetUpForeBlackTitleBar();
                }
            }
        }
        #endregion

        #region Drawer
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

        #region Animations
        private void ToggleAddingAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : -(float)this.ActualWidth);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _addingPanelVisual.StartAnimation("Offset.x", offsetAnimation);
        }

        private void SwitchToDeleteCommandBar()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 50f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);

            var offsetAnimation2 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation2.InsertKeyFrame(1f, 0f);
            offsetAnimation2.Duration = TimeSpan.FromMilliseconds(300);
            offsetAnimation2.DelayTime = TimeSpan.FromMilliseconds(200);

            _defaultCommandBarVisual.StartAnimation("Offset.y", offsetAnimation);
            _deleteComamndBarVisual.StartAnimation("Offset.y", offsetAnimation2);
        }

        private void SwitchToDefaultCommandBar()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 0f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(300);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(200);

            var offsetAnimation2 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation2.InsertKeyFrame(1f, 50f);
            offsetAnimation2.Duration = TimeSpan.FromMilliseconds(300);

            _defaultCommandBarVisual.StartAnimation("Offset.y", offsetAnimation);
            _deleteComamndBarVisual.StartAnimation("Offset.y", offsetAnimation2);
        }

        private void ToggleHeaderAnimation(bool showHamburger)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, showHamburger ? 0 : -30f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, showHamburger ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(800);

            HamburgerBtn.IsEnabled = true;
            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _headerVisual.StartAnimation("Offset.x", offsetAnimation);
            _hamburgerVisual.StartAnimation("Opacity", fadeAnimation);
            batch.Completed += ((sender, e) =>
              {
                  if (!showHamburger)
                  {
                      HamburgerBtn.IsEnabled = false;
                  }
                  else
                  {
                      HamburgerBtn.IsEnabled = true;
                  }
              });
            batch.End();
        }

        private void ToggleAnimationWithAddingPanel(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0 : -100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            var offsetAnimation2 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation2.InsertKeyFrame(1f, show ? 0 : -150f);
            offsetAnimation2.Duration = TimeSpan.FromMilliseconds(800);

            var offsetAnimation3 = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation3.InsertKeyFrame(1f, show ? 0 : 50f);
            offsetAnimation3.Duration = TimeSpan.FromMilliseconds(800);

            _contentRootGirdVisual.StartAnimation("Offset.x", offsetAnimation2);
            _defaultCommandBarVisual.StartAnimation("Offset.y", offsetAnimation3);

            if (this.ActualWidth >= WIDTH_THRESHOLD)
                _drawerVisual.StartAnimation("Offset.x", offsetAnimation);
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
            Grid.SetColumnSpan(titleBarUC, 5);
            Grid.SetRowSpan(titleBarUC, 5);
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

        private void HeaderSP_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sp = sender as StackPanel;
            sp.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
        }

        private async void DisplayedListView_OnReorderStopped()
        {
            await MainVM.UpdateOrderAsync();
        }
    }
}
