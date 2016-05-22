using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MyerListUWP.ViewModel;

namespace MyerList.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<SettingPageViewModel>();
        }

        public SettingPageViewModel SettingVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingPageViewModel>();
            }
        }

        public static void Cleanup()
        {

        }
    }
}
