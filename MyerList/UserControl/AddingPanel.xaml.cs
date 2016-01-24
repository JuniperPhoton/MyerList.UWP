using MyerList.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


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
        }

        public void SetFocus()
        {
            AddContentBox.Focus(FocusState.Programmatic);
        }
    }
}
