using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using System;
using MyerList.Model;
using MyerListUWP.Common;
using JP.Utils.Helper;
using MyerListUWP;

namespace MyerList.UC
{
    public sealed partial class ScheduleControl : UserControl
    {
        TranslateTransform _tranTemplete = new TranslateTransform();
        bool _isToBeDone = false;
        bool _isToBeDeleted = false;

        public ToDo CurrentToDo
        {
            get
            {
                return this.DataContext as ToDo;
            }
        }

        public ScheduleControl()
        {
            this.InitializeComponent();

            BackStory.Completed += ((senderb, eb) =>
              {
                  InitialManipulation();
              });
            this.Loaded += ScheduleControl_Loaded;
        }

        private void ScheduleControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.MainVM.CanBeSorted)
            {
                c2.Width = new GridLength(0);
            }
            else
            {
                c2.Width = new GridLength(50);
            }
        }

        private void InitialManipulation()
        {
            SchduleTempleteGrid.ManipulationDelta -= Grid_ManipulationDelta;
            SchduleTempleteGrid.ManipulationCompleted -= Grid_ManipulationCompleted;

            SchduleTempleteGrid.ManipulationDelta += Grid_ManipulationDelta;
            SchduleTempleteGrid.ManipulationCompleted += Grid_ManipulationCompleted;

            _tranTemplete = new TranslateTransform();
            SchduleTempleteGrid.RenderTransform = _tranTemplete;
        }

        /// <summary>
        /// 启动动画
        /// </summary>
        /// <param name="x">当前的X位置</param>
        private void BeginReturnStoryboard(double x)
        {
            StartX.Value = x;
            BackStory.Begin();
        }

        private void SchduleTempleteGrid_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            InitialManipulation();
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            //完成待办事项 
            if (e.Delta.Translation.X > 0)
            {
                _tranTemplete.X += e.Delta.Translation.X;
                if (_isToBeDeleted)
                {
                    return;
                }
                if (_tranTemplete.X > 100)
                {
                    if (!_isToBeDone)
                    {
                        ShowGreenStory.Begin();
                        _isToBeDone = true;
                        _isToBeDeleted = false;
                    }
                }
            }
            //删除待办事项
            else
            {
                _tranTemplete.X += e.Delta.Translation.X;
                if (_isToBeDone)
                {
                    return;
                }
                if (_tranTemplete.X < -100)
                {
                    if (!_isToBeDeleted)
                    {
                        ShowRedStory.Begin();
                        _isToBeDone = false;
                        _isToBeDeleted = true;
                    }
                }
            }
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X > 0)
            {
                if (e.Cumulative.Translation.X > 100)
                {
                   Messenger.Default.Send(new GenericMessage<ToDo>(this.DataContext as ToDo),MessengerTokens.CheckToDo);
                }
                HideGreenStory.Begin();
                BeginReturnStoryboard(e.Cumulative.Translation.X);
            }
            else if (e.Cumulative.Translation.X < 0)
            {
                if (e.Cumulative.Translation.X < -100)
                {
                    if (SchduleTempleteGrid != null)
                    {
                        Messenger.Default.Send(new GenericMessage<ToDo>(this.DataContext as ToDo), MessengerTokens.DeleteToDo);
                    }
                }
                HideRedStory.Begin();
                BeginReturnStoryboard(e.Cumulative.Translation.X);
            }
            _isToBeDone = false;
            _isToBeDeleted = false;
        }

        private void SchduleTempleteGrid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                try
                {
                    var attatchedFlyout = FlyoutBase.GetAttachedFlyout(element) as MenuFlyout;
                    var position = e.GetPosition(null);
                    attatchedFlyout.ShowAt(null, position);
                }
                catch (Exception)
                {

                }
            }
        }

        private void SchduleTempleteGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if(DeviceHelper.IsDesktop)
            {
                FrameworkElement element = sender as FrameworkElement;
                if (element != null)
                {
                    try
                    {
                        var attatchedFlyout = FlyoutBase.GetAttachedFlyout(element) as MenuFlyout;
                        var position = e.GetPosition(null);
                        attatchedFlyout.ShowAt(null, position);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        private void MarkDownItem_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new GenericMessage<ToDo>(CurrentToDo), MessengerTokens.CheckToDo);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send(new GenericMessage<ToDo>(CurrentToDo), MessengerTokens.DeleteToDo);
        }
    }
}
