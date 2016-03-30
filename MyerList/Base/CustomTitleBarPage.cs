using JP.Utils.Helper;
using MyerList.UC;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyerList.Base
{
    public class CustomTitleBarPage : BindablePage
    {
        protected TitleBarUC TitleBarUC;

        public CustomTitleBarPage()
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(DeviceHelper.IsDesktop)
                CustomTitleBar();
        }

        private void CustomTitleBar()
        {
            var currentContent = this.Content;
            TitleBarUC = new TitleBarUC();
            TitleBarUC.BackHandler += ((sender, e) =>
              {
                  if (Frame.CanGoBack) Frame.GoBack();
              });
            (currentContent as Grid).Children.Add(TitleBarUC);
        }
    }
}
