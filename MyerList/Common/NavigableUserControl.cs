using CompositionHelper;
using JP.Utils.Helper;
using MyerList.Helper;
using MyerList.Interface;
using MyerList.UC;
using MyerListUWP.Helper;
using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerList.Common
{
    public class ShownArgs
    {
        public bool Shown { get; set; }
    }

    public class NavigableUserControl : UserControl, INavigableUserControl
    {
        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(NavigableUserControl),
                new PropertyMetadata(false, OnShownPropertyChanged));

        public event EventHandler<ShownArgs> OnShownChanged;

        private static void OnShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as INavigableUserControl;
            if ((bool)e.NewValue) control.OnShow();
            else control.OnHide();
            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _rootVisual;

        private TitleBarControl _titleBarControl;

        public NavigableUserControl()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                this.SizeChanged += UserControlBase_SizeChanged;
            }
        }

        private void UserControlBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetOffset();
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _rootVisual = this.GetVisual();
            ResetOffset();
        }

        private void ResetOffset()
        {
            if (!Shown)
            {
                _rootVisual.Offset = new Vector3(0f, (float)ActualHeight, 0f);
            }
        }

        public virtual void OnHide()
        {
            OnShownChanged?.Invoke(this, new ShownArgs() { Shown = false });
        }

        public virtual void OnShow()
        {
            OnShownChanged?.Invoke(this, new ShownArgs() { Shown = true });
            _titleBarControl = TitleBarHelper.CustomTitleBar(this.Content);
            _titleBarControl.OnClickBackBtn += (x, e) =>
            {
                Shown = false;
            };
            //Window.Current.SetTitleBar(this);
            if (DeviceHelper.IsMobile)
            {
                StatusBarHelper.SetUpBlackStatusBar();
            }
            else
            {
                TitleBarHelper.SetUpTitleBarColorForDarkText();
            }
        }

        public void ToggleAnimation()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, Shown ? 0f : (float)ActualHeight);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            _rootVisual.StartAnimation("Offset.y", offsetAnimation);
        }
    }
}
