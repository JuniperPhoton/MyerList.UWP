using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using System;
using MyerList.Model;
using MyerListUWP.Common;
using JP.Utils.Helper;
using MyerListUWP;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using System.Numerics;
using System.Threading.Tasks;

namespace MyerList.UC
{
    public sealed partial class ScheduleControl : UserControl
    {
        private bool _isToBeDone = false;
        private bool _isToBeDeleted = false;

        private Compositor _compositor;
        private Visual _rootVisual;

        private ToDo CurrentToDo
        {
            get
            {
                return this.DataContext as ToDo;
            }
        }

        public ScheduleControl()
        {
            this.InitializeComponent();
            this.Loaded += ScheduleControl_Loaded;
            this.InitComposition();
            LeftGrid.ManipulationStarted += RootGrid_ManipulationStarted;
            LeftGrid.ManipulationDelta += Grid_ManipulationDelta;
            LeftGrid.ManipulationCompleted += Grid_ManipulationCompleted;
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _rootVisual = ElementCompositionPreview.GetElementVisual(RootGrid);
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

        private void RootGrid_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Handled = true;
            App.MainVM.EnableItemClick = false;
        }

        private void ToggleBackAnimation()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 0f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _rootVisual.StartAnimation("Offset.X", offsetAnimation);
            batch.Completed += async(sender, e) =>
              {
                  await Task.Delay(500);
                  App.MainVM.EnableItemClick = true;
              };
            batch.End();
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            var x = _rootVisual.Offset.X;
            _rootVisual.Offset = new Vector3((float)(x + e.Delta.Translation.X), 0f, 0f);

            //完成待办事项
            if (_rootVisual.Offset.X > 0)
            {
                if (_isToBeDeleted)
                {
                    return;
                }
                if (_rootVisual.Offset.X > 100)
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
                if (_isToBeDone)
                {
                    return;
                }
                if (_rootVisual.Offset.X < -100)
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
            e.Handled = true;
            App.MainVM.EnableItemClick = false;

            if (e.Cumulative.Translation.X > 0)
            {
                if (e.Cumulative.Translation.X > 100)
                {
                    Messenger.Default.Send(new GenericMessage<ToDo>(CurrentToDo), MessengerTokens.CheckToDo);
                }
                HideGreenStory.Begin();
            }
            else if (e.Cumulative.Translation.X < 0)
            {
                if (e.Cumulative.Translation.X < -100)
                {
                    if (RootGrid != null)
                    {
                        Messenger.Default.Send(new GenericMessage<ToDo>(CurrentToDo), MessengerTokens.DeleteToDo);
                    }
                }
                HideRedStory.Begin();
            }
            _isToBeDone = false;
            _isToBeDeleted = false;

            ToggleBackAnimation();
        }

        private void RootGrid_Holding(object sender, HoldingRoutedEventArgs e)
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

        private void RootGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (DeviceHelper.IsDesktop)
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

        private void RootGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
