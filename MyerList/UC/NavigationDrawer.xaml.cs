using MyerList.ViewModel;
using Windows.UI.Xaml.Controls;

namespace MyerList.UC
{
    public sealed partial class NavigationDrawer : UserControl
    {
        public MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public NavigationDrawer()
        {
            this.InitializeComponent();
        }
    }
}
