using GalaSoft.MvvmLight.Messaging;
using MyerList.Base;
using MyerList.ViewModel;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;


namespace MyerList
{
    public sealed partial class StartPage : BindablePage
    {
        private StartViewModel StartVM;
        private Compositor _compositor;
        private Visual _logoVisual;
        private Visual _nameVisual;
        private Visual _subTitleVisual;
        private Visual _loginBtnVisual;
        private Visual _registerBtnVisual;
        private Visual _offlineBtnVisual;

        private List<Visual> _visualList = new List<Visual>();

        public StartPage()
        {
            this.InitializeComponent();

            this.DataContext = StartVM = new StartViewModel();
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _logoVisual = ElementCompositionPreview.GetElementVisual(LogoImage);
            _nameVisual = ElementCompositionPreview.GetElementVisual(NameSP);
            _subTitleVisual = ElementCompositionPreview.GetElementVisual(SubtitleTB);
            _loginBtnVisual = ElementCompositionPreview.GetElementVisual(LoginBtn);
            _registerBtnVisual = ElementCompositionPreview.GetElementVisual(RegisterBtn);
            _offlineBtnVisual = ElementCompositionPreview.GetElementVisual(OfflineBtn);

            _visualList = new List<Visual>();
            _visualList.Add(_logoVisual);
            _visualList.Add(_nameVisual);
            _visualList.Add(_subTitleVisual);
            _visualList.Add(_loginBtnVisual);
            _visualList.Add(_registerBtnVisual);
            _visualList.Add(_offlineBtnVisual);

            this.Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            _visualList.ForEach(s =>
            {
                s.Offset = new Vector3((float)Window.Current.Bounds.Width / 4f, 0f, 0f);
                s.Opacity = 0;
            });

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, 0f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, 1f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(1000);

            for (int i = 0; i < _visualList.Count; i++)
            {
                var visual = _visualList[i];
                offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(i * 50);
                visual.StartAnimation("Offset.X", offsetAnimation);
                visual.StartAnimation("Opacity", fadeAnimation);
            }
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpForeBlackTitleBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TitleBarHelper.SetUpForeBlackTitleBar();
            Frame.BackStack.Clear();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }
    }
}
