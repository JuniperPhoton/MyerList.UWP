using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using JP.Utils.Helper;
using MyerList.Base;
using MyerList.Helper;
using MyerList.Model;
using MyerList.ViewModel;
using MyerListCustomControl;
using MyerListUWP.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

        public MainPage()
        {
            this.InitializeComponent();

            if (LocalSettingHelper.HasValue(ResourcesHelper.GetDicString("FeatureToken")))
            {
                if (LocalSettingHelper.GetValue(ResourcesHelper.GetDicString("FeatureToken")) == "true")
                {
                    FeatureGrid.Visibility = Visibility.Collapsed;
                }
                else FeatureGrid.Visibility = Visibility.Visible;
            }
            else FeatureGrid.Visibility = Visibility.Visible;

            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowPaneOpen"),
            };
            BindingOperations.SetBinding(this, IsAddingPaneOpenProperty, b);

            RemoveStory.Completed += ((senderr, er) =>
              {

              });
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

            RegisterMessenger();

            if (DeviceHelper.IsMobile)
            {
                var b2 = new Binding()
                {
                    Source=this.MainVM,
                    Path=new PropertyPath("CateColor"),
                    Mode=BindingMode.OneWay
                };
                //BindingOperations.SetBinding(this.ContentRootGird, BackgroundProperty, b2);
                TitleTB.Foreground = new SolidColorBrush(Colors.White);
                HamburgerBtn.ForegroundBrush = new SolidColorBrush(Colors.White);
            }
        }

        public static async void IsAddingPaneOpenPropertyChanged(DependencyObject d,DependencyPropertyChangedEventArgs args)
        {
            var page = d as MainPage;
            if (args.NewValue == args.OldValue) return;
            if ((bool)args.NewValue) await page.EnterAddMode();
            else page.LeaveAddmode();
        }

        private void RegisterMessenger()
        {
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.CloseHam, msg =>
            {
                if (_isDrawerSlided && CoreWindow.GetForCurrentThread().Bounds.Width < 720)
                {
                    SlideOutStory.Begin();
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
            if (args.Size.Width >= 720)
            {
                left = 270;
                right = (args.Size.Width - 250) / 5d;
                _isDrawerSlided = true;
            }
            else _isDrawerSlided = false;
            ListContentGrid.Margin = new Thickness(left, 0, right, 0);
            HeaderContentGrid.Margin = new Thickness(left, 0, right, 0);
        }

        #endregion

        #region CommandBar
        private async Task EnterAddMode()
        {
            AddingPane.Visibility = Visibility.Visible;
            AddStory.Begin();
            if (_isDrawerSlided)
            {
                SlideOutStory.Begin();
            }
            await Task.Delay(100);
            AddingPane.SetFocus();
        }

        private void LeaveAddmode()
        {
            RemoveStory.Begin();
            if (CoreWindow.GetForCurrentThread().Bounds.Width >= 720)
            {
                SlideInStory.Begin();
                _isDrawerSlided = true;
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
        }

        private void MaskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_isDrawerSlided)
            {
                _isDrawerSlided = false;
                HamburgerBtn.PlayHamOutStory();
                SlideOutStory.Begin();
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
            if (_pointOriX < 20 && AppSettings.Instance.EnableGesture && !IsAddingPaneOpen)
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
                HamburgerBtn.PlayHamInStory();
            }
            else if (transform.TranslateX <= -200)
            {
                _isDrawerSlided = false;
                SlideOutKey1.Value = transform.TranslateX;
                SlideOutKey2.Value = MaskBorder.Opacity;
                SlideOutStory.Begin();
                HamburgerBtn.PlayHamOutStory();
            }
        }

        #endregion

        #region Override

        protected override void SetUpTitleBarExtend()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }


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

            Frame.BackStack.Clear();
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
        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            FeatureGrid.Visibility = Visibility.Collapsed;
            LocalSettingHelper.AddValue(ResourcesHelper.GetDicString("FeatureToken"), "true");
        }
        #endregion
    }
}
