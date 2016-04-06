using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Helper;
using Lousy.Mon;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using MyerList.Base;
using MyerList.Helper;
using MyerList.UC;
using MyerList.ViewModel;
using MyerListCustomControl;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using System;
using System.Numerics;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.UI.Composition.Toolkit;
using System.Threading.Tasks;

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

        #region DP
        //添加/修改的面板是否出现
        public bool IsAddingPanelOpen
        {
            get { return (bool)GetValue(IsAddingPanelOpenProperty); }
            set { SetValue(IsAddingPanelOpenProperty, value); }
        }

        public static DependencyProperty IsAddingPanelOpenProperty =
            DependencyProperty.Register("IsAddingPanelOpen", typeof(bool), typeof(MainPage), new PropertyMetadata(false,
               (sender, e) =>
               {
                   var page = sender as MainPage;
                   if (e.NewValue == e.OldValue) return;
                   if ((bool)e.NewValue) page.ShowAddingPanel();
                   else page.HideAddingPanel();
               }));
        #endregion

        //当前抽屉是否出现了
        private bool _isDrawerSlided = false;

        //当前的手指位置，用于手势
        private double _pointOriX = 0;

        #region Composition vars
        private Compositor _compositor;
        private CompositionImageFactory _imageLoader;
        private CompositionImage _circleMaskSurface;
        private SpriteVisual _coloredCircleVisual;
        private ContainerVisual _containerVisual;
        private Visual _addingPaneVisual;
        private Visual _drawerVisual;
        private Visual _contentVisual;
        private Visual _rootVisual;
        #endregion

        public MainPage()
        {
            this.InitializeComponent();

            SlideOutStory.Completed += ((senders, es) =>
              {
                  MaskBorder.Visibility = Visibility.Collapsed;
                  SlideOutKey1.Value = 0;
                  SlideOutKey2.Value = 0.5;
                  _isDrawerSlided = false;
              });
            SlideInStory.Completed += ((sendere, ee) =>
              {
                  SlideInKey1.Value = -260;
                  SlideInKey2.Value = 0;
                  _isDrawerSlided = true;
              });

            CoreWindow.GetForCurrentThread().SizeChanged += MainPage_SizeChanged;

            MainVM.OnCateColorChanged += MainVM_OnCategoryChanged;

            RegisterMessenger();
            InitialLayout();
            InitialCompositor();
            InitialBinding();
            InitialFeature();
        }

        private void InitialFeature()
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

        private void InitialBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowPanelOpen"),
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, IsAddingPanelOpenProperty, b);
        }

        private void InitialCompositor()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _addingPaneVisual = ElementCompositionPreview.GetElementVisual(AddingPanel);
            _addingPaneVisual.Offset = new Vector3(0f, 50f, 0);
            _addingPaneVisual.Opacity = 0;
            _drawerVisual = ElementCompositionPreview.GetElementVisual(Drawer);
            _contentVisual = ElementCompositionPreview.GetElementVisual(ContentRootGird);
            _drawerVisual.AnchorPoint = new Vector2(0.5f, 0.5f);
            _contentVisual.AnchorPoint = new Vector2(0.5f, 0.5f);
            _rootVisual = ElementCompositionPreview.GetElementVisual(VisualGrid);
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
                    SlideOutStory.Begin();
                    MaskOutStory.Begin();
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
                    NarrowToWideStory.Begin();
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
                    WideToNarrowStory.Begin();
                }
                HamburgerBtn.ForegroundBrush = new SolidColorBrush(Colors.White);
                TitleTB.Foreground = new SolidColorBrush(Colors.White);
                ProgressRing.Foreground = new SolidColorBrush(Colors.White);
                TitleBarHelper.SetUpForeWhiteTitleBar();
            }
        }

        #endregion

        #region CommandBar
        private async Task KickOffCircleVisualAnimation()
        {

            if (_containerVisual == null)
            {
                _containerVisual = _compositor.CreateContainerVisual();

                ElementCompositionPreview.SetElementChildVisual(VisualGrid, _containerVisual);

                //Load image
                _imageLoader = CompositionImageFactory.CreateCompositionImageFactory(_compositor);
                _circleMaskSurface = _imageLoader.CreateImageFromUri(new Uri("ms-appx:///Assets/Icon/CircleOpacityMask.png"));
            }

            _containerVisual.Children.RemoveAll();

            var point = AddBtn.TransformToVisual(VisualGrid).TransformPoint(new Point(AddBtn.ActualWidth / 2, AddBtn.ActualHeight / 2));

            // create the visual with a solid colored circle as brush
            _coloredCircleVisual = _compositor.CreateSpriteVisual();
            _coloredCircleVisual.Brush = CreateCircleBrushWithColor(App.MainVM.AddingCateColor.Color);
            _coloredCircleVisual.Offset = new Vector3((float)point.X, (float)point.Y, 1f);
            _coloredCircleVisual.Size = new Vector2(50, 50);

            // we want our scale animation to be anchored around the center of the visual
            _coloredCircleVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

            _containerVisual.Children.InsertAtTop(_coloredCircleVisual);

            var radius = CircleVisualHelper.GetLongestRadius(point);

            var animation = CircleVisualHelper.CreateBoomAnim(_compositor, true, (float)radius / 25f);

            _coloredCircleVisual.StartAnimation("Scale.X", animation);
            _coloredCircleVisual.StartAnimation("Scale.Y", animation);

            await Task.Delay(300);

            ShowOrHideAddingPane(true);
            AddingPanel.SetFocus();

            TitleBarHelper.SetUpForeWhiteTitleBar();
        }

        private CompositionEffectBrush CreateCircleBrushWithColor(Color color)
        {
            var colorBrush = _compositor.CreateColorBrush(color);

            //
            // Because Windows.UI.Composition does not have a Circle visual, we will 
            // work around by using a circular opacity mask
            // Create a simple Composite Effect, using DestinationIn (S * DA), 
            // with a color source and a named parameter source.
            //
            var effect = new CompositeEffect
            {
                Mode = CanvasComposite.DestinationIn,
                Sources =
                {
                    new ColorSourceEffect()
                    {
                        Color = color
                    },
                    new CompositionEffectSourceParameter("mask")
                }

            };
            var factory = _compositor.CreateEffectFactory(effect);
            var brush = factory.CreateBrush();

            //
            // Create the mask brush using the circle mask
            //
            CompositionSurfaceBrush maskBrush = _compositor.CreateSurfaceBrush();
            maskBrush.Surface = _circleMaskSurface.Surface;

            brush.SetSourceParameter("mask", maskBrush);

            return brush;
        }

        private void ShowOrHideAddingPane(bool show)
        {
            AddingPanel.Visibility = Visibility.Visible;

            var offsetYAnim = _compositor.CreateScalarKeyFrameAnimation();
            offsetYAnim.InsertKeyFrame(1f, show ? 0f : 50f);
            offsetYAnim.Duration = TimeSpan.FromMilliseconds(500);

            var fadeAnim = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnim.InsertKeyFrame(1f, show ? 1f : 0f);
            fadeAnim.Duration = TimeSpan.FromMilliseconds(500);

            _addingPaneVisual.StartAnimation("Opacity", fadeAnim);
            _addingPaneVisual.StartAnimation("Offset.Y", offsetYAnim);
        }

        private async void ShowAddingPanel()
        {
            await KickOffCircleVisualAnimation();
        }

        private void HideAddingPanel()
        {
            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _coloredCircleVisual.StartAnimation("Scale.X", CircleVisualHelper.CreateBoomAnim(_compositor, false, null));
            _coloredCircleVisual.StartAnimation("Scale.Y", CircleVisualHelper.CreateBoomAnim(_compositor, false, null));

            ShowOrHideAddingPane(false);

            batch.End();
            batch.Completed += ((sender, e) =>
              {
                  AddingPanel.Visibility = Visibility.Collapsed;
              });

            if (Window.Current.Bounds.Width >= 720)
            {
                TitleBarHelper.SetUpForeBlackTitleBar();
            }
        }
        #endregion

        #region Drawer
        private void HamClick(object sender, RoutedEventArgs e)
        {
            _isDrawerSlided = true;
            MaskBorder.Visibility = Visibility.Visible;
            HamburgerBtn.PlayHamInStory();
            SlideInStory.Begin();
            FadeMaskStory(true);
        }

        private void MaskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isDrawerSlided)
            {
                _isDrawerSlided = false;
                HamburgerBtn.PlayHamOutStory();
                SlideOutStory.Begin();
                FadeMaskStory(false);
            }
        }

        private void Grid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (DeviceHelper.IsDesktop) return;
            _pointOriX = e.Position.X;
            MaskBorder.Visibility = Visibility.Visible;
            MaskBorder.Opacity = 0;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (DeviceHelper.IsDesktop) return;
            if (_pointOriX < 20 && AppSettings.Instance.EnableGesture && !IsAddingPanelOpen)
            {
                var transform = Drawer.RenderTransform as CompositeTransform;
                var newX = transform.TranslateX + e.Delta.Translation.X;
                if (newX < 0) transform.TranslateX = newX;
                if (newX > -100) _isDrawerSlided = true;

                if (MaskBorder.Opacity < 0.8) MaskBorder.Opacity += e.Delta.Translation.X > 0 ? 0.01 : -0.01;
            }
        }

        private void TouchGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (DeviceHelper.IsDesktop) return;
            var transform = Drawer.RenderTransform as CompositeTransform;
            if (transform.TranslateX < 0 && transform.TranslateX > -200)
            {
                _isDrawerSlided = true;
                SlideInKey1.Value = transform.TranslateX;
                SlideInKey2.Value = 0.8;
                SlideInStory.Begin();
                MaskInStory.Begin();
                HamburgerBtn.PlayHamInStory();
            }
            else if (transform.TranslateX <= -200)
            {
                _isDrawerSlided = false;
                SlideOutKey1.Value = transform.TranslateX;
                SlideOutKey2.Value = MaskBorder.Opacity;
                SlideOutStory.Begin();
                MaskOutStory.Begin();
                HamburgerBtn.PlayHamOutStory();
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
            if (ApiInformationHelper.HasHardwareButton)
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        protected override void UnRegisterHandleBackLogic()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= NewMainPage_BackRequested;
            if (ApiInformationHelper.HasHardwareButton)
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
            if (MainVM.ShowPanelOpen)
            {
                MainVM.ShowPanelOpen = false;
                return true;
            }
            if (MainVM.SelectedCate != 0)
            {
                MainVM.SelectedCate = 0;
                return true;
            }
            if (_isDrawerSlided)
            {
                SlideOutStory.Begin();
                HamburgerBtn.PlayHamOutStory();
                _isDrawerSlided = false;
                return true;
            }
            return false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (AppSettings.Instance.EnableBackgroundTask)
            {
                BackgroundTaskHelper.RegisterBackgroundTask();
            }
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
                SlideOutStory.Begin();
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

        #region Composition Anim
        private void FadeMaskStory(bool isIn)
        {
            var root = ElementCompositionPreview.GetElementVisual(MaskBorder);
            var anim = _compositor.CreateScalarKeyFrameAnimation();
            anim.InsertExpressionKeyFrame(1f, "isIn?0.8f:0f");
            anim.Duration = TimeSpan.FromMilliseconds(1500);
            root.StartAnimation("Opacity", anim);
        }
        #endregion

        #region List anim
        private void DisplayedListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            // Don't run an entrance animation if we're in recycling
            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }
        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsStackPanel)DisplayedListView.ItemsPanelRoot;
            var itemContainer = (ListViewItem)sender;
            var itemIndex = DisplayedListView.IndexFromContainer(itemContainer);

            var uc = itemContainer.ContentTemplateRoot as UIElement;

            // Don't animate if we're not in the visible viewport
            if (itemIndex >= itemsPanel.FirstVisibleIndex && itemIndex <= itemsPanel.LastVisibleIndex)
            {
                var itemVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                float width = (float)uc.RenderSize.Width;
                float height = (float)uc.RenderSize.Height;
                itemVisual.CenterPoint = new Vector3(width / 2, height / 2, 0f);
                itemVisual.Opacity = 0f;
                itemVisual.Offset = new Vector3(0, 50, 0);

                // Create KeyFrameAnimations
                KeyFrameAnimation offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                offsetAnimation.InsertExpressionKeyFrame(1f, "0");
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);
                offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 100);

                KeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertExpressionKeyFrame(1f, "1");
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(1000);
                fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 100);

                // Start animations
                itemVisual.StartAnimation("Offset.Y", offsetAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }
            itemContainer.Loaded -= ItemContainer_Loaded;
        }
        #endregion
    }
}
