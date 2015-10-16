using GalaSoft.MvvmLight.Messaging;
using JP.Utils.Data;
using MyerList.Base;
using MyerList.Helper;
using MyerList.Model;
using MyerList.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Phone.UI.Input;
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
        public MainViewModel MainVM
        {
            get
            {
                return DataContext as MainViewModel;
            }
        }

        private bool _isDrawerSlided = false;

        private double _pointOriX = 0;

        public bool IsAddingPaneOpen
        {
            get { return (bool)GetValue(IsAddingPaneOpenProperty); }
            set { SetValue(IsAddingPaneOpenProperty, value); }
        }

        public static DependencyProperty IsAddingPaneOpenProperty =
            DependencyProperty.Register("IsAddingPaneOpen", typeof(bool), typeof(MainPage), new PropertyMetadata(false,
               new PropertyChangedCallback((sender, e) =>
              {
                  var page = sender as MainPage;
                  if (e.NewValue == e.OldValue) return;
                  if ((bool)e.NewValue) page.EnterAddMode();
                  else page.LeaveAddmode();
              })));

        public MainPage()
        {
            this.InitializeComponent();

            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowPaneOpen"),
            };
            BindingOperations.SetBinding(this, IsAddingPaneOpenProperty, b);

            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.ToastToken, msg =>
            {
                ToastControl.ShowMessage(msg.Content);
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.CloseHam, msg =>
            {
                if (_isDrawerSlided)
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
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.GoToSort, act =>
            {
                DisplayedListView.CanDragItems = true;
                DisplayedListView.CanReorderItems = true;
                DisplayedListView.AllowDrop = true;
                DisplayedListView.IsItemClickEnabled = false;

                ToastControl.ShowMessage(ResourcesHelper.GetString("ReorderHint"));
            });
            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.LeaveSort, act =>
            {
                DisplayedListView.CanDragItems = false;
                DisplayedListView.CanReorderItems = false;
                DisplayedListView.AllowDrop = false;
                DisplayedListView.IsItemClickEnabled = true;
            });
            RemoveStory.Completed += ((senderr, er) =>
              {
                  //AddingPane.Visibility = Visibility.Collapsed;
              });
            SlideOutStory.Completed += ((senders, es) =>
              {
                  MaskBorder.Visibility = Visibility.Collapsed;
                  SlideOutKey1.Value = 0;
                  SlideOutKey2.Value = 0.5;
              });
            SlideInStory.Completed += ((sendere, ee) =>
              {
                  SlideInKey1.Value = -260;
                  SlideInKey2.Value = 0;
              });
        }

        protected override void SetUpTitleBarExtend()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        #region CommandBar
        private void EnterAddMode()
        {
            AddingPane.Visibility = Visibility.Visible;
            AddStory.Begin();
            AddingPane.SetFocus();
        }

        private void LeaveAddmode()
        {
            RemoveStory.Begin();
        }
        #endregion

        #region 汉堡按钮

        private void HamClick(object sender, RoutedEventArgs e)
        {
            _isDrawerSlided = true;
            MaskBorder.Visibility = Visibility.Visible;
            HamburgerBtn.PlayHamInStory();
            SlideInStory.Begin();
        }

        #endregion

        #region 手势打开

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
            _pointOriX = e.Position.X;
            MaskBorder.Visibility = Visibility.Visible;
            MaskBorder.Opacity = 0.1;
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_pointOriX < 10 && LocalSettingHelper.GetValue("EnableGesture") == "true")
            {
                var transform = Drawer.RenderTransform as CompositeTransform;
                var newX = transform.TranslateX + e.Delta.Translation.X;
                if(newX<0) transform.TranslateX = newX;
                if (newX > -100) _isDrawerSlided = true;

                if(MaskBorder.Opacity<0.5) MaskBorder.Opacity += e.Delta.Translation.X > 0 ? 0.05 : -0.05;
            }
        }

        private void TouchGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var transform = Drawer.RenderTransform as CompositeTransform;
            if(transform.TranslateX<0 && transform.TranslateX>-200)
            {
                _isDrawerSlided = true;
                SlideInKey1.Value = transform.TranslateX;
                SlideInKey2.Value = 0.5;
                SlideInStory.Begin();
                HamburgerBtn.PlayHamInStory();
            }
            else if(transform.TranslateX<=-200)
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
        protected override void SetNavigationBackBtn()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        protected override void RegisterHandleBackLogic()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += NewMainPage_BackRequested;
            if (ApiInformationHelper.HasHardwareButton())
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }
        }

        protected override void UnRegisterHandleBackLogic()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= NewMainPage_BackRequested;
            if (ApiInformationHelper.HasHardwareButton())
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

            if (LocalSettingHelper.GetValue("EnableBackgroundTask") == "true")
            {
                BackgroundTaskHelper.RegisterBackgroundTask(DeviceKind.Windows);
            }

            Frame.BackStack.Clear();


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (_isDrawerSlided)
            {
                SlideOutStory.Begin();
                HamburgerBtn.PlayHamOutStory();
                _isDrawerSlided = false;
            }
        }

        #endregion

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            List<ToDo> list = new List<ToDo>();
            foreach(var item in MainVM.CurrentDisplayToDos)
            {
                var newToDo = new ToDo();
                newToDo.IsDone = item.IsDone;
                newToDo.Content = item.Content;
                list.Add(newToDo);
            }
            MultiWindowsHelper.ListToDisplayInNewWindow = list;
            await MultiWindowsHelper.ActiveOrCreateNewWindow(MainVM.SelectedCate,true);
        }
    }
}
