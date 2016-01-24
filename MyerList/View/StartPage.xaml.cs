using GalaSoft.MvvmLight.Messaging;
using MyerList.Base;
using MyerList.ViewModel;
using MyerListUWP;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace MyerList
{

    public sealed partial class StartPage : BindablePage
    {
        public StartViewModel StartVM;

        public StartPage()
        {
            this.InitializeComponent();
            StartVM = new StartViewModel();
            this.DataContext = StartVM;
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpWideTitleBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            TitleBarHelper.SetUpWideTitleBar();
            Frame.BackStack.Clear();
            
            StartStory.BeginTime = TimeSpan.FromSeconds(0.2);
            StartStory2.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GenericMessage<string>(""), "ClearInfo");
        }
    }
}
