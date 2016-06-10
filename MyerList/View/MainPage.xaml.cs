using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Helper;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using MyerList.Base;
using MyerList.Helper;
using MyerList.Model;
using MyerList.UC;
using MyerList.ViewModel;
using MyerListCustomControl;
using MyerListUWP.Common;
using MyerListUWP.Helper;
using SamplesCommon.ImageLoader;
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
        private Visual _holderVisual;

        private ContainerVisual _containerForVisuals;
        private SpriteVisual _colorVisual;
        private ScalarKeyFrameAnimation _bloomAnimation;
        private IImageLoader _imageLoader;
        private IManagedSurface _circleMaskSurface;

        public MainPage()
        {
            this.InitializeComponent();

            if (!DesignMode.DesignModeEnabled)
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
            _holderVisual = ElementCompositionPreview.GetElementVisual(VisualHolder);
            _containerForVisuals = _compositor.CreateContainerVisual();

            _drawerMaskVisual.Opacity = 0;
            _drawerVisual.Offset = new Vector3(-250, 0f, 0f);
            _deleteComamndBarVisual.Offset = new Vector3(0f, 50f, 0f);

            _addingPanelVisual.Offset = new Vector3(0f, 100f, 0f);
            _addingPanelVisual.Opacity = 0;

            _containerForVisuals = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(VisualHolder, _containerForVisuals);

            // initialize the ImageLoader and create the circle mask
            _imageLoader = ImageLoaderFactory.CreateImageLoader(_compositor);
            _circleMaskSurface = _imageLoader.CreateManagedSurfaceFromUri(new Uri("ms-appx:///Assets/Icon/CircleOpacityMask.png"));

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

        #region Public API surface
        public void StartColorAnimation(Color color, Rect initialBounds, Rect finalBounds)
        {
            _colorVisual = CreateVisualWithColorAndPosition(color, initialBounds, finalBounds);

            _containerForVisuals.Children.RemoveAll();
            // add our solid colored circle visual to the live visual tree via the container
            _containerForVisuals.Children.InsertAtTop(_colorVisual);

            // now that we have a visual, let's run the animation 
            TriggerBloomAnimation(_colorVisual);
        }

        /// <summary>
        /// Cleans up any remaining surfaces.
        /// </summary>
        public void DisposeSurfaces()
        {
            _circleMaskSurface.Dispose();
        }

        #endregion

        #region All the heavy lifting
        /// <summary>
        /// Creates a Visual using the specific color and constraints
        /// </summary>
        private SpriteVisual CreateVisualWithColorAndPosition(Color color,
                                                              Rect initialBounds,
                                                              Rect finalBounds)
        {

            // init the position and dimensions for our visual
            var width = (float)initialBounds.Width;
            var height = (float)initialBounds.Height;
            var positionX = initialBounds.X;
            var positionY = initialBounds.Y;

            // we want our visual (a circle) to completely fit within the bounding box
            var circleColorVisualDiameter = (float)Math.Min(width, height);

            // the diameter of the circular visual is an essential bit of information
            // in initializing our bloom animation - a one-time thing
            InitializeBloomAnimation(circleColorVisualDiameter / 2, finalBounds); // passing in the radius

            // we are going to some lengths to have the visual precisely placed
            // such that the center of the circular visual coincides with the center of the AppBarButton.
            // it is important that the bloom originate there
            var diagonal = Math.Sqrt(2 * (circleColorVisualDiameter * circleColorVisualDiameter));
            var deltaForOffset = (diagonal - circleColorVisualDiameter) / 2;

            // now we have everything we need to calculate the position (offset) and size of the visual
            var offset = new Vector3((float)positionX + (float)deltaForOffset + circleColorVisualDiameter / 2,
                                     (float)positionY + circleColorVisualDiameter / 2,
                                     0f);
            var size = new Vector2(circleColorVisualDiameter);

            // create the visual with a solid colored circle as brush
            SpriteVisual coloredCircleVisual = _compositor.CreateSpriteVisual();
            coloredCircleVisual.Brush = CreateCircleBrushWithColor(color);
            coloredCircleVisual.Offset = offset;
            coloredCircleVisual.Size = size;

            // we want our scale animation to be anchored around the center of the visual
            coloredCircleVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

            return coloredCircleVisual;
        }


        /// <summary>
        /// Creates a circular solid colored brush that we can apply to a visual
        /// </summary>
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

        /// <summary>
        /// Creates an animation template for a "color bloom" type effect on a circular colored visual.
        /// This is a sub-second animation on the Scale property of the visual.
        /// 
        /// <param name="initialRadius">the Radius of the circular visual</param>
        /// <param name="finalBounds">the final area to occupy</param>
        /// </summary>
        private void InitializeBloomAnimation(float initialRadius, Rect finalBounds)
        {
            var maxWidth = finalBounds.Width;
            var maxHeight = finalBounds.Height;

            // when fully scaled, the circle must cover the entire viewport
            // so we use the window's diagonal width as our max radius, assuming 0,0 placement
            var maxRadius = (float)Math.Sqrt((maxWidth * maxWidth) + (maxHeight * maxHeight)); // hypotenuse

            // the scale factor is the ratio of the max radius to the original radius
            var scaleFactor = (float)Math.Round(maxRadius / initialRadius, MidpointRounding.AwayFromZero);


            var bloomEase = _compositor.CreateCubicBezierEasingFunction(  //these numbers seem to give a consistent circle even on small sized windows
                    new Vector2(0.1f, 0.4f),
                    new Vector2(0.99f, 0.65f)
                );
            _bloomAnimation = _compositor.CreateScalarKeyFrameAnimation();
            _bloomAnimation.InsertKeyFrame(1.0f, scaleFactor);
            _bloomAnimation.Duration = TimeSpan.FromMilliseconds(500); // keeping this under a sec to not be obtrusive

        }

        private ScalarKeyFrameAnimation InitializeBloomDismissAnimation()
        {
            var anim = _compositor.CreateScalarKeyFrameAnimation();
            anim.InsertKeyFrame(1.0f, 0.01f);
            anim.Duration = TimeSpan.FromMilliseconds(500); // keeping this under a sec to not be obtrusive
            return anim;
        }

        /// <summary>
        /// Runs the animation
        /// </summary>
        private void TriggerBloomAnimation(SpriteVisual colorVisual)
        {

            // animate the Scale of the visual within a scoped batch
            // this gives us transactionality and allows us to do work once the transaction completes
            var batchTransaction = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            // as with all animations on Visuals, these too will run independent of the UI thread
            // so if the UI thread is busy with app code or doing layout on state/page transition,
            // these animations still run uninterruped and glitch free
            colorVisual.StartAnimation("Scale.X", _bloomAnimation);
            colorVisual.StartAnimation("Scale.Y", _bloomAnimation);

            batchTransaction.End();
        }

        #endregion

        #region SizeChanged
        bool _isToggleAnim1 = false;
        bool _isToggleAnim2 = false;

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e?.NewSize.Width > WIDTH_THRESHOLD && e.PreviousSize.Width <= WIDTH_THRESHOLD && !_isDrawerSlided)
            {
                SwitchToWideStory.Begin();
                ToggleDrawerAnimation(true);
                _isDrawerSlided = true;
                ChangeCommandBarColorToWhiteStory.Begin();
                ToggleHeaderAnimation(false);
            }
            else if (e?.NewSize.Width <= WIDTH_THRESHOLD && e?.PreviousSize.Width > WIDTH_THRESHOLD && _isDrawerSlided)
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
                //ToggleAnimationWithAddingPanel(false);
                AddingPanel.SetFocus();
            }
            else
            {
                var dismissAnimation = InitializeBloomDismissAnimation();
                if (_colorVisual != null)
                {
                    var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    _colorVisual.StartAnimation("Scale.x", dismissAnimation);
                    _colorVisual.StartAnimation("Scale.y", dismissAnimation);
                    batch.Completed += (sender, e) =>
                    {
                        _containerForVisuals.Children.RemoveAll();
                    };
                    batch.End();
                }

                ToggleAddingAnimation(false);
                //ToggleAnimationWithAddingPanel(true);

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
            AddingPanel.Visibility = Visibility.Visible;

            var targetOffsetX = AddBtn.TransformToVisual(this).TransformPoint(new Point(0, 0));
            StartColorAnimation(MainVM.CateColor.Color,
                new Rect(targetOffsetX.X, targetOffsetX.Y, 50d, 50d), new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : 100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(300);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(300);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _addingPanelVisual.StartAnimation("Offset.y", offsetAnimation);
            _addingPanelVisual.StartAnimation("Opacity", fadeAnimation);
            batch.Completed += (sender, e) =>
              {
                  if (!show) AddingPanel.Visibility = Visibility.Collapsed;
              };
            batch.End();
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
