using MyerList.Base;
using MyerList.Converter;
using MyerList.Helper;
using MyerList.Model;
using MyerList.ViewModel;
using MyerListUWP;
using MyerListUWP.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace MyerList.View
{
    public sealed partial class CardViewPage : BindablePage,INotifyPropertyChanged
    {
        int CurrentCate = -1;

        IEnumerable<ToDo> SrcList;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if(handler!=null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<ToDo> _currentList;
        public ObservableCollection<ToDo> CurrentList
        {
            get
            {
                return _currentList;
            }
            set
            {
                if (_currentList != value)
                {
                    _currentList = value;
                    RaisePropertyChanged(nameof(CurrentList));
                }
            }
        }

        public CardViewPage()
        {
            this.InitializeComponent();

            var appView = ApplicationView.GetForCurrentView();
            appView.Consolidated += AppView_Consolidated;
            Window.Current.Activated += Current_Activated; ;
            
            CurrentList = new ObservableCollection<ToDo>();
            this.DataContext = this;

            NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            CurrentList = new ObservableCollection<ToDo>();
            var mainVM = (App.Current.Resources["Locator"] as ViewModelLocator).MainVM;
            if (CurrentCate == -1) CurrentList = mainVM.CurrentDisplayToDos;
            else
            {
                var list = from item in mainVM.MyToDos where item.Category == CurrentCate select item;
                list.ToList().ForEach(s => CurrentList.Add(s));
            }
        }

        private void AppView_Consolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args)
        {
            MultiWindowsHelper.ViewIDs.Remove(CurrentCate);
        }

        private void SetUpData(MultiWindowsData data)
        {
            //CurrentList.Clear();
            //data.CurrentDisplayList.ToList().ForEach(s => CurrentList.Add(s));
            //RaisePropertyChanged(nameof(CurrentList));

            CurrentCate = data.CateColor;
            SrcList = data.CurrentDisplayList;

            if (data.CurrentDisplayList.Count()==0)
            {
                WorkDoneSP.Visibility = Visibility.Visible;
            }

            var cate = data.CateColor;
            var color = new CateColorConverter().Convert(cate, null, null, null);
            TitleGrid.Background = (SolidColorBrush)color;

            switch (cate)
            {
                case 0:
                    {
                        TitleTB.Text = ResourcesHelper.GetString("CateDefault");
                    }; break;
                case 1:
                    {
                        TitleTB.Text = ResourcesHelper.GetString("CateWork");

                    }; break;
                case 2:
                    {
                        TitleTB.Text = ResourcesHelper.GetString("CateLife");

                    }; break;
                case 3:
                    {
                        TitleTB.Text = ResourcesHelper.GetString("CateFamily");
                    }; break;
                case 4:
                    {
                        TitleTB.Text = ResourcesHelper.GetString("CateEnter");
                    }; break;
            }
        }

        protected override void SetNavigationBackBtn()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(e.Parameter is MultiWindowsData)
            {
                SetUpData(e.Parameter as MultiWindowsData);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
           
        }
    }
}
