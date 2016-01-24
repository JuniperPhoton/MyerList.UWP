using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace FABExt
{
    public enum ManipulatingMode
    {
        None,
        Up,
        Left,
    }

    public class FABExt : ContentControl
    {
        #region DP
        public ICommand PrimaryCommand
        {
            get { return (ICommand)GetValue(PrimaryCommandProperty); }
            set { SetValue(PrimaryCommandProperty, value); }
        }

        public static readonly DependencyProperty PrimaryCommandProperty =
            DependencyProperty.Register("PrimaryCommand", typeof(ICommand), typeof(FABExt), new PropertyMetadata(null));

        public ICommand SecondaryUpCommand
        {
            get { return (ICommand)GetValue(SecondaryUpCommandProperty); }
            set { SetValue(SecondaryUpCommandProperty, value); }
        }

        public static readonly DependencyProperty SecondaryUpCommandProperty =
            DependencyProperty.Register("SecondaryUpCommand", typeof(ICommand), typeof(FABExt), new PropertyMetadata(null));

        public ICommand SecondaryLeftCommand
        {
            get { return (ICommand)GetValue(SecondaryLeftCommandProperty); }
            set { SetValue(SecondaryLeftCommandProperty, value); }
        }

        public static readonly DependencyProperty SecondaryLeftCommandProperty =
            DependencyProperty.Register("SecondaryLeftCommand", typeof(ICommand), typeof(FABExt), new PropertyMetadata(null));

        public SolidColorBrush PrimaryColor
        {
            get { return (SolidColorBrush)GetValue(PrimaryColorProperty); }
            set { SetValue(PrimaryColorProperty, value); }
        }

        public static readonly DependencyProperty PrimaryColorProperty =
            DependencyProperty.Register("PrimaryColor", typeof(SolidColorBrush), typeof(FABExt), new PropertyMetadata(new SolidColorBrush(Colors.Cyan)));

        public SolidColorBrush SecondaryColor
        {
            get { return (SolidColorBrush)GetValue(SecondaryColorProperty); }
            set { SetValue(SecondaryColorProperty, value); }
        }

        public static readonly DependencyProperty SecondaryColorProperty =
            DependencyProperty.Register("SecondaryColor", typeof(SolidColorBrush), typeof(FABExt), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public UIElement SecondaryUpContent
        {
            get { return (UIElement)GetValue(SecondaryUpContentProperty); }
            set { SetValue(SecondaryUpContentProperty, value); }
        }

        public static readonly DependencyProperty SecondaryUpContentProperty =
            DependencyProperty.Register("SecondaryUpContent", typeof(UIElement), typeof(FABExt), new PropertyMetadata(new Grid()));

        public UIElement SecondaryLeftContent
        {
            get { return (UIElement)GetValue(SecondaryLeftContentProperty); }
            set { SetValue(SecondaryLeftContentProperty, value); }
        }

        public static readonly DependencyProperty SecondaryLeftContentProperty =
            DependencyProperty.Register("SecondaryLeftContent", typeof(UIElement), typeof(FABExt), new PropertyMetadata(new Grid()));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(FABExt), new PropertyMetadata(50d));

        #endregion

        public event Action<FABExt> OnClickPrimaryButton;
        public event Action<FABExt> OnClickSecondaryUpButton;
        public event Action<FABExt> OnClickSecondaryLeftButton;

        private TaskCompletionSource<int> _tcs;
        private ManipulatingMode _mode = ManipulatingMode.None;

        private double TRANSLATE_LIMIT
        {
            get
            {
                return -this.Radius * 1.5;
            }
        }

        #region Control
        private Button _primaryBtn;
        private Button _secondaryUpBtn;
        private Button _secondaryLeftBtn;
        private Border _maskBorder;
        #endregion

        public FABExt()
        {
            DefaultStyleKey = typeof(FABExt);
            _tcs = new TaskCompletionSource<int>();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _primaryBtn = GetTemplateChild("PrimaryBtn") as Button;
            _secondaryUpBtn = GetTemplateChild("SecondaryUpBtn") as Button;
            _secondaryLeftBtn = GetTemplateChild("SecondaryLeftBtn") as Button;
            _maskBorder = GetTemplateChild("MaskBorder") as Border;
            _tcs.TrySetResult(0);

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            _primaryBtn.Click += _primaryBtn_Click;
            _secondaryUpBtn.Click += _secondaryUpBtn_Click;
            _secondaryLeftBtn.Click += _secondaryLeftBtn_Click;

            _primaryBtn.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            _primaryBtn.ManipulationDelta += _primaryBtn_ManipulationDelta;
            _primaryBtn.ManipulationCompleted += _primaryBtn_ManipulationCompleted;

            _maskBorder.Tapped += _maskBorder_Tapped;
        }

        private void _secondaryLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickSecondaryLeftButton?.Invoke(this);
            SecondaryLeftCommand.Execute(null);
            var sb2 = MakeScaleAndDispearStory(_secondaryLeftBtn, TimeSpan.FromSeconds(0.5));
            sb2.Completed += ((e1, e2) =>
              {
                  ResetTransform();
              });
            sb2.Begin();

            var sb1 = MakeOpacityStory(_maskBorder, _maskBorder.Opacity, 0, TimeSpan.FromSeconds(0.3));
            sb1.Completed += ((e1, e2) =>
            {
                _maskBorder.Visibility = Visibility.Collapsed;
            });
            sb1.Begin();
        }

        private void _secondaryUpBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickSecondaryUpButton?.Invoke(this);
            SecondaryUpCommand.Execute(null);
            var sb2 = MakeScaleAndDispearStory(_secondaryUpBtn, TimeSpan.FromSeconds(0.5));
            sb2.Completed += ((e1, e2) =>
            {
                ResetTransform();
            });
            sb2.Begin();

            var sb1 = MakeOpacityStory(_maskBorder, _maskBorder.Opacity, 0, TimeSpan.FromSeconds(0.3));
            sb1.Completed += ((e1, e2) =>
            {
                _maskBorder.Visibility = Visibility.Collapsed;
            });
            sb1.Begin();
        }

        private void _primaryBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickPrimaryButton?.Invoke(this);
        }

        private void _maskBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var sb1 = MakeOpacityStory(_maskBorder, 1, 0, TimeSpan.FromSeconds(0.3));
            sb1.Begin();

            var transformUp = _secondaryUpBtn.RenderTransform as CompositeTransform;
            if (transformUp.TranslateY < 0)
            {
                var sb = MakeTranslateStory(_secondaryUpBtn, TRANSLATE_LIMIT, 0, TimeSpan.FromSeconds(0.5), false);
                sb.Begin();
            }

            var transformLeft = _secondaryLeftBtn.RenderTransform as CompositeTransform;
            if (transformLeft.TranslateX < 0)
            {
                var sb = MakeTranslateStory(_secondaryLeftBtn, TRANSLATE_LIMIT, 0, TimeSpan.FromSeconds(0.5), true);
                sb.Begin();
            }
        }

        private void ResetTransform()
        {
            var transformUp = _secondaryUpBtn.RenderTransform as CompositeTransform;
            transformUp.ScaleX = 0;
            transformUp.ScaleY = 0;
            transformUp.TranslateY = 0;
            _secondaryUpBtn.Opacity = 1;

            var transformLeft = _secondaryLeftBtn.RenderTransform as CompositeTransform;
            transformLeft.ScaleX = 0;
            transformLeft.ScaleY = 0;
            transformLeft.TranslateX = 0;
            _secondaryLeftBtn.Opacity = 1;
        }

        private void _primaryBtn_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_mode == ManipulatingMode.None)
            {
                if (e.Cumulative.Translation.Y < -10) _mode = ManipulatingMode.Up;
                else if (e.Cumulative.Translation.X < -10) _mode = ManipulatingMode.Left;
            }

            if (_mode == ManipulatingMode.Up)
            {
                var transform = _secondaryUpBtn.RenderTransform as CompositeTransform;
                var resultY = (transform.TranslateY + e.Delta.Translation.Y) * 1.02;
                if (resultY > TRANSLATE_LIMIT && resultY < 0) transform.TranslateY = resultY;

                HandleScale(transform, transform.TranslateY);
            }
            else if (_mode == ManipulatingMode.Left)
            {
                var transform = _secondaryLeftBtn.RenderTransform as CompositeTransform;
                var resultX = (transform.TranslateX + e.Delta.Translation.X) * 1.02;
                if (resultX > TRANSLATE_LIMIT && resultX < 0) transform.TranslateX = resultX;

                HandleScale(transform, transform.TranslateX);
            }
        }

        private void HandleScale(CompositeTransform transform, double currentTranslateValue)
        {
            var resultScale = (Math.Abs(currentTranslateValue / TRANSLATE_LIMIT)) * 1.5;
            if (resultScale >= 0 && resultScale <= 1)
            {
                transform.ScaleX = resultScale;
                transform.ScaleY = resultScale;
                _maskBorder.Visibility = Visibility.Visible;
                _maskBorder.Opacity = resultScale;
            }
        }

        private void _primaryBtn_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var acrossLimit = false;
            if (_mode == ManipulatingMode.Up)
            {
                var transform = _secondaryUpBtn.RenderTransform as CompositeTransform;
                acrossLimit = (transform.TranslateY > TRANSLATE_LIMIT && transform.TranslateY <= TRANSLATE_LIMIT / 4);

                var sb = MakeTranslateStory(_secondaryUpBtn, transform.TranslateY, acrossLimit ? TRANSLATE_LIMIT : 0, TimeSpan.FromSeconds(0.3), false);
                sb.Begin();

                if (transform.ScaleX < 1)
                {
                    var sb2 = MakeScaleStory(_secondaryUpBtn, transform.ScaleX, acrossLimit ? 1 : 0, TimeSpan.FromSeconds(0.3));
                    sb2.Completed += ((e1, e2) =>
                      {
                          if (acrossLimit) _secondaryUpBtn_Click(null, null);
                      });
                    sb2.Begin();
                }
            }
            else if (_mode == ManipulatingMode.Left)
            {
                var transform = _secondaryLeftBtn.RenderTransform as CompositeTransform;
                acrossLimit = (transform.TranslateX > TRANSLATE_LIMIT && transform.TranslateX <= TRANSLATE_LIMIT / 4);

                var sb = MakeTranslateStory(_secondaryLeftBtn, transform.TranslateX, acrossLimit ? TRANSLATE_LIMIT : 0, TimeSpan.FromSeconds(0.3), true);
                sb.Begin();
                if (transform.ScaleX < 1)
                {
                    var sb2 = MakeScaleStory(_secondaryLeftBtn, transform.ScaleX, acrossLimit ? 1 : 0, TimeSpan.FromSeconds(0.3));
                    sb2.Completed += ((e1, e2) =>
                    {
                        if(acrossLimit) _secondaryLeftBtn_Click(null, null);
                    });
                    sb2.Begin();
                }
            }
            var opacity = _maskBorder.Opacity;
            if (opacity < 1)
            {
                var sb1 = MakeOpacityStory(_maskBorder, opacity, acrossLimit ? 1 : 0, TimeSpan.FromSeconds(0.3));
                sb1.Begin();
            }
            _mode = ManipulatingMode.None;

        }

        #region Uti
        public static Storyboard MakeOpacityStory(DependencyObject element, double fromValue, double toValue, TimeSpan lastTime)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames, "Opacity");
            Storyboard.SetTarget(frames, element);

            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame();
            frame1.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame1.Value = fromValue;

            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame();
            frame2.KeyTime = lastTime;
            frame2.Value = toValue;

            frames.KeyFrames.Add(frame1);
            frames.KeyFrames.Add(frame2);

            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.Children.Add(frames);

            return sb;
        }

        public static Storyboard MakeTranslateStory(DependencyObject element, double fromValue, double toValue, TimeSpan lastTime, bool isX)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames, isX ? "(UIElement.RenderTransform).(CompositeTransform.TranslateX)" : "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
            Storyboard.SetTarget(frames, element);

            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame();
            frame1.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame1.Value = fromValue;

            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame();
            frame2.KeyTime = lastTime;
            frame2.Value = toValue;
            frame2.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };

            frames.KeyFrames.Add(frame1);
            frames.KeyFrames.Add(frame2);

            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.Children.Add(frames);

            return sb;
        }

        public static Storyboard MakeScaleStory(DependencyObject element, double fromValue, double toValue, TimeSpan lastTime)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames frames1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(frames1, element);

            EasingDoubleKeyFrame frame11 = new EasingDoubleKeyFrame();
            frame11.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame11.Value = fromValue;

            EasingDoubleKeyFrame frame12 = new EasingDoubleKeyFrame();
            frame12.KeyTime = lastTime;
            frame12.Value = toValue;

            frames1.KeyFrames.Add(frame11);
            frames1.KeyFrames.Add(frame12);

            DoubleAnimationUsingKeyFrames frames2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(frames2, element);

            EasingDoubleKeyFrame frame21 = new EasingDoubleKeyFrame();
            frame21.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame21.Value = fromValue;

            EasingDoubleKeyFrame frame22 = new EasingDoubleKeyFrame();
            frame22.KeyTime = lastTime;
            frame22.Value = toValue;

            frames2.KeyFrames.Add(frame21);
            frames2.KeyFrames.Add(frame22);

            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.Children.Add(frames1);
            sb.Children.Add(frames2);

            return sb;
        }

        public static Storyboard MakeScaleAndDispearStory(DependencyObject element, TimeSpan lastTime)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimationUsingKeyFrames frames = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames, "Opacity");
            Storyboard.SetTarget(frames, element);

            EasingDoubleKeyFrame frame1 = new EasingDoubleKeyFrame();
            frame1.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame1.Value = 1;

            EasingDoubleKeyFrame frame2 = new EasingDoubleKeyFrame();
            frame2.KeyTime = lastTime;
            frame2.Value = 0;

            frames.KeyFrames.Add(frame1);
            frames.KeyFrames.Add(frame2);

            DoubleAnimationUsingKeyFrames frames1 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames1, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            Storyboard.SetTarget(frames1, element);

            EasingDoubleKeyFrame frame11 = new EasingDoubleKeyFrame();
            frame11.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame11.Value = 1;

            EasingDoubleKeyFrame frame12 = new EasingDoubleKeyFrame();
            frame12.KeyTime = lastTime;
            frame12.Value = 5;

            frames1.KeyFrames.Add(frame11);
            frames1.KeyFrames.Add(frame12);

            DoubleAnimationUsingKeyFrames frames2 = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(frames2, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(frames2, element);

            EasingDoubleKeyFrame frame21 = new EasingDoubleKeyFrame();
            frame21.KeyTime = new TimeSpan(0, 0, 0, 0);
            frame21.Value = 1;

            EasingDoubleKeyFrame frame22 = new EasingDoubleKeyFrame();
            frame22.KeyTime = lastTime;
            frame22.Value = 5;

            frames2.KeyFrames.Add(frame21);
            frames2.KeyFrames.Add(frame22);

            sb.FillBehavior = FillBehavior.HoldEnd;
            sb.Children.Add(frames1);
            sb.Children.Add(frames2);
            sb.Children.Add(frames);
            return sb;
        }
        #endregion
    }
}
