using JP.Utils.Helper;
using MyerList.UC;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyerList.Base
{
    public class CustomTitleBarPage : BindablePage
    {
        protected TitleBarControl TitleBarUC;

        public CustomTitleBarPage()
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(DeviceHelper.IsDesktop)
            {
                CustomTitleBar();
            }
        }

        private void CustomTitleBar()
        {
            var currentContent = this.Content as Grid;
            if(currentContent==null)
            {
                throw new ArgumentNullException("The root element of the page should be Grid.");
            }
            TitleBarUC = new TitleBarControl();
            TitleBarUC.OnClickBackBtn += ((sender, e) =>
              {
                  if (Frame.CanGoBack) Frame.GoBack();
              });
            (currentContent as Grid).Children.Add(TitleBarUC);
            Grid.SetColumnSpan(TitleBarUC, 5);
            Grid.SetRowSpan(TitleBarUC, 5);
        }
    }
}
