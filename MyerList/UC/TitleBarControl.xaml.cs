using JP.Utils.Helper;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyerList.UC
{
    public sealed partial class TitleBarControl : UserControl
    {
        public RoutedEventHandler OnClickBackBtn;

        public TitleBarControl()
        {
            this.InitializeComponent();
            if (DeviceHelper.IsDesktop)
                Window.Current.SetTitleBar(backGrdRect);
        }

        public void SetForegroundColor(Color color)
        {
            if (DeviceHelper.IsDesktop)
            {
                BackSymbol.Foreground = new SolidColorBrush(color);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            OnClickBackBtn?.Invoke(sender, e);
        }
    }
}
