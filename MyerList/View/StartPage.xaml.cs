using GalaSoft.MvvmLight.Messaging;
using JP.Utils.CompositionAPI;
using MyerList.Base;
using MyerList.ViewModel;
using MyerListUWP.Helper;
using System;
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
        public StartViewModel StartVM;

        private Compositor _compositor;
        private Visual _heroImgVisual;
        private Visual _nameSPVisual;
        private Visual _subTitleVisual;
        private Visual _loginBtnVisual;
        private Visual _registerBtnVisual;
        private Visual _offlineBtnVisual;
        private Visual _loginControlVisual;

        private bool _isInLoginMode = false;

        public StartPage()
        {
            this.InitializeComponent();
            StartVM = new StartViewModel();
            this.DataContext = StartVM;
            this.Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _heroImgVisual = ElementCompositionPreview.GetElementVisual(HeroImage);
            _nameSPVisual = ElementCompositionPreview.GetElementVisual(NameSP);
            _subTitleVisual = ElementCompositionPreview.GetElementVisual(SubtitleTB);
            _loginBtnVisual = ElementCompositionPreview.GetElementVisual(LoginBtn);
            _registerBtnVisual = ElementCompositionPreview.GetElementVisual(RegisterBtn);
            _offlineBtnVisual = ElementCompositionPreview.GetElementVisual(OfflineBtn);
            _loginControlVisual = ElementCompositionPreview.GetElementVisual(LoginControl);

            _loginControlVisual.Opacity = 0;
            _loginControlVisual.Offset = new Vector3(500f, 0f, 0f);

            KickOffStartElementAnimation(true);
        }

        private void KickOffStartElementAnimation(bool isIn)
        {
            InitStartElementAnimation(_heroImgVisual, 0, isIn);
            InitStartElementAnimation(_nameSPVisual, 1, isIn);
            InitStartElementAnimation(_subTitleVisual, 2, isIn);
            InitStartElementAnimation(_loginBtnVisual, 3, isIn);
            InitStartElementAnimation(_registerBtnVisual, 4, isIn);
            InitStartElementAnimation(_offlineBtnVisual, 5, isIn);
        }

        private void InitStartElementAnimation(Visual visual, int delayIndex, bool isIn)
        {
            visual.Opacity =isIn? 0:1;
            visual.Offset = new Vector3(isIn?_isInLoginMode?-200f:200f:0f, 0f, 0f);

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, isIn ? 0f : -200f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(delayIndex * 50);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, isIn ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(1000);
            fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(delayIndex * 50);

            visual.StartAnimation("Offset.x", offsetAnimation);
            visual.StartAnimation("Opacity", fadeAnimation);
        }

        private void KickOffLoginElementAnimation(bool isIn)
        {
            InitLoginElementAnimation(_loginControlVisual, isIn);
        }

        private void InitLoginElementAnimation(Visual visual, bool isIn)
        {
            visual.Opacity = isIn ? 0 : 1;
            visual.Offset = new Vector3(isIn ?500f : 0f, 0f, 0f);

            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, isIn ? 0f : 500f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(1000);

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, isIn ? 1f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(1000);

            visual.StartAnimation("Offset.x", offsetAnimation);
            visual.StartAnimation("Opacity", fadeAnimation);
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

        private  async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            KickOffStartElementAnimation(false);
            await Task.Delay(600);
            KickOffLoginElementAnimation(true);
            _isInLoginMode = true;
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            KickOffStartElementAnimation(false);
            await Task.Delay(600);
            KickOffLoginElementAnimation(true);
            _isInLoginMode = true;
        }

        private async void LoginControl_OnClickBackBtn()
        {
            KickOffLoginElementAnimation(false);
            await Task.Delay(600);
            KickOffStartElementAnimation(true);
            _isInLoginMode = false;
        }
    }
}
