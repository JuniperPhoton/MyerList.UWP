using JP.Utils.Helper;
using MyerList.ViewModel;
using MyerListUWP;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyerList.UC
{
    public sealed partial class AddingPanel : UserControl
    {
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
        }

        public async void SetFocus()
        {
            AddContentBox.Focus(FocusState.Programmatic);

            await Task.Delay(500);
            AddContentBox.Select(AddContentBox.Text.Length, 0);
        }

        private void AddContentBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                App.MainVM.OkCommand.Execute(null);
            }
        }
    }
}
