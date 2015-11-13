using JP.Utils.Helper;
using MyerList.UC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
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
            var oldContent = this.Content;
            TitleBarUC = new TitleBarUC();
            TitleBarUC.BackHandler += ((sender, e) =>
              {
                  if (Frame.CanGoBack) Frame.GoBack();
              });
            (oldContent as Grid).Children.Add(TitleBarUC);
        }
    }
}
