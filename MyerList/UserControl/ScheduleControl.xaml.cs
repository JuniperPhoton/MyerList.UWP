using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using MyerList.Helper;

namespace MyerList.UC
{
    public sealed partial class ScheduleControl : UserControl
    {

        TranslateTransform _tranTemplete = new TranslateTransform();
        bool _isToBeDone = false;
        bool _isInDeleteMode = false;
        bool _canBeSorted = false;

        ManipulationModes defaultmode=ManipulationModes.TranslateX | ManipulationModes.System;
        ManipulationModes reordermode = ManipulationModes.TranslateY;

        public ScheduleControl()
        {
            this.InitializeComponent();

            Messenger.Default.Register<GenericMessage<string>>(this, MessengerTokens.GoToSort, act =>
                {
                    if(!_canBeSorted)
                    {
                        _canBeSorted = true;
                        GoSortStory.Begin();
                        SchduleTempleteGrid.ManipulationMode = reordermode;
                        //LeftSP.IsHitTestVisible = false;
                    }
                });
            Messenger.Default.Register<GenericMessage<string>>(this,MessengerTokens.LeaveSort, act =>
            {
                if (_canBeSorted)
                {
                    _canBeSorted = false;
                    LeaveSortStory.Begin();
                    SchduleTempleteGrid.ManipulationMode = defaultmode;
                    LeftSP.IsHitTestVisible = true;
                }
            });

            BackStory.Completed += ((senderb, eb) =>
              {
                  InitialManipulation();
              });
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
                if (_isInDeleteMode)
                {
                    return;
                }
                if (_tranTemplete.X > 100)
                {
                    if (!_isToBeDone)
                    {
                        ShowGreenStory.Begin();
                        _isToBeDone = true;
                        _isInDeleteMode = false;
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
                    if (!_isInDeleteMode)
                    {
                        ShowRedStory.Begin();
                        _isToBeDone = false;
                        _isInDeleteMode = true;
                    }
                }
            }

        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Grid _grid = sender as Grid;

            if (e.Cumulative.Translation.X > 10)
            {
                if (e.Cumulative.Translation.X > 100)
                {
                   Messenger.Default.Send(new GenericMessage<string>((string)_grid.Tag),MessengerTokens.CheckToDo);
                }
                HideGreenStory.Begin();
                BeginReturnStoryboard(e.Cumulative.Translation.X);
            }
            else if (e.Cumulative.Translation.X < -10)
            {
                if (e.Cumulative.Translation.X < -100)
                {
                    if (_grid != null)
                        Messenger.Default.Send(new GenericMessage<string>((string)_grid.Tag), MessengerTokens.DeleteToDo);
                }
                HideRedStory.Begin();
                BeginReturnStoryboard(e.Cumulative.Translation.X);
            }
            _isToBeDone = false;
            _isInDeleteMode = false;
        }

    }
}
