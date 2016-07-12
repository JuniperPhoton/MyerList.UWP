using MyerList.ViewModel;
using MyerListUWP;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyerList.UC
{
    public sealed partial class NavigationDrawer : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public SolidColorBrush BackgrdColor
        {
            get { return (SolidColorBrush)GetValue(BackgrdColorProperty); }
            set { SetValue(BackgrdColorProperty, value); }
        }

        public static readonly DependencyProperty BackgrdColorProperty =
            DependencyProperty.Register("BackgrdColor", typeof(SolidColorBrush), typeof(NavigationDrawer),
                new PropertyMetadata((SolidColorBrush)App.Current.Resources["DefaultColor"], OnBackgrdColorPropertyChanged));

        private static void OnBackgrdColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var drawer = d as NavigationDrawer;
            drawer.StartColorAnimation((SolidColorBrush)e.NewValue);
        }

        public NavigationDrawer()
        {
            this.InitializeComponent();
        }

        private void StartColorAnimation(SolidColorBrush targetColorBrush)
        {
            this.ColorAnimation.To = targetColorBrush.Color;
            ColorStory.Begin();
        }
    }
}
