using GalaSoft.MvvmLight.Messaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using MyerList.Base;
using MyerList.Helper;
using MyerList.ViewModel;
using MyerListUWP;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MyerList
{

    public sealed partial class LoginPage : CustomTitleBarPage
    {
        public LoginViewModel LoginVM;

        public LoginPage()
        {
            this.InitializeComponent();
            this.KeyDown += LoginPage_KeyDown;

            LoginVM = new LoginViewModel();
            this.DataContext = LoginVM;

            if(ApiInformationHelper.HasStatusBar())
            {
                StatusBar.GetForCurrentView().BackgroundColor = (App.Current.Resources["MyerListBlueLight"] as SolidColorBrush).Color;
                StatusBar.GetForCurrentView().BackgroundOpacity = 0.01;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            }

            Messenger.Default.Register<GenericMessage<string>>(this, "toast", act =>
            {
                var msg = act.Content;
                ToastControl.ShowMessage(msg);
            });
        }

        //#region BackGrdImage

        //private CanvasBitmap bitmapTiger;
        //private ICanvasImage effect;

        //private void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        //{
        //    args.TrackAsyncAction(Canvas_CreateResourcesAsync(sender).AsAsyncAction());
        //}

        //private async Task Canvas_CreateResourcesAsync(CanvasControl sender)
        //{
        //    bitmapTiger = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/loginbackgrd.jpg"));

        //    effect = CreateGaussianBlur();
        //}

        //private ICanvasImage CreateGaussianBlur()
        //{
        //    var blurEffect = new GaussianBlurEffect
        //    {
        //        Source = bitmapTiger,
        //        BorderMode = EffectBorderMode.Hard,
        //        BlurAmount =0
        //    };
        //    return blurEffect;
        //}
        //private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        //{
        //    var size = sender.Size;
        //    var scale = size.Width / size.Height;
        //    var ds = args.DrawingSession;

        //    ds.DrawImage(effect,
        //        new Rect(0, 0, size.Width, size.Height),
        //        new Rect(0, 0,
        //            size.Width,
        //            size.Height));
        //}

        //private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (canvas.ReadyToDraw)
        //    {
        //        canvas.Invalidate();
        //    }
        //}

        //#endregion

        private void LoginPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Messenger.Default.Send(new GenericMessage<string>(""), MessengerTokens.PressEnterToLoginToken);
                this.Focus(FocusState.Pointer);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }
    }
}
