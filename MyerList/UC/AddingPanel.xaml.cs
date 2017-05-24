using JP.Utils.Helper;
using MyerList.ViewModel;
using MyerListUWP;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyerList.UC
{
    public sealed partial class AddingPanel : UserControl
    {
        public SolidColorBrush BackgrdColor
        {
            get { return (SolidColorBrush)GetValue(BackgrdColorProperty); }
            set { SetValue(BackgrdColorProperty, value); }
        }

        public static readonly DependencyProperty BackgrdColorProperty =
            DependencyProperty.Register("BackgrdColor", typeof(SolidColorBrush), typeof(AddingPanel),
                new PropertyMetadata((SolidColorBrush)App.Current.Resources["MyerListBlue"], OnBackgrdColorPropertyChanged));

        private static void OnBackgrdColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AddingPanel;
            control.UpdateBackgrdColor((SolidColorBrush)e.NewValue);
        }

        public MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public AddingPanel()
        {
            this.InitializeComponent();

            if (DeviceHelper.IsMobile)
            {
                RootSP.Margin = new Thickness(0, 100, 0, 0);
                RootSP.VerticalAlignment = VerticalAlignment.Top;
            }

            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("AddingCateColor"),
                Mode = BindingMode.OneWay,
            };
            this.SetBinding(BackgrdColorProperty, b);
        }

        public async void SetFocus()
        {
            AddContentBox.Focus(FocusState.Programmatic);

            await Task.Delay(500);
            AddContentBox.Select(AddContentBox.Text.Length, 0);
        }

        private void UpdateBackgrdColor(SolidColorBrush targetColorBrush)
        {
            ColorAnimation.To = targetColorBrush.Color;
            ChangeColorStory.Begin();
        }

        private void AddContentBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                App.MainVM.OkCommand.Execute(null);
            }
        }

        private void AddGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < 0)
            {
                var currentIndex = MainVM.AddingCate;
                if (currentIndex == MainVM.CateVM.Categories.Count - 2)
                {
                    currentIndex = 0;
                }
                else currentIndex++;
                CateListBox.SelectedIndex = currentIndex;
            }
            else if (e.Cumulative.Translation.X > 0)
            {
                var currentIndex = MainVM.AddingCate;
                if (currentIndex == 0)
                {
                    currentIndex = MainVM.CateVM.Categories.Count - 2;
                }
                else currentIndex--;
                CateListBox.SelectedIndex = currentIndex;
            }
        }
    }
}
