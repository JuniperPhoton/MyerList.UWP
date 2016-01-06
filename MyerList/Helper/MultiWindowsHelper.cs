using MyerList.Model;
using MyerList.View;
using MyerListUWP.Common;
using MyerListUWP.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerList.Helper
{
    public class MultiWindowsHelper
    {
        public static Dictionary<int, int> ViewIDs = new Dictionary<int, int>();

        public static IEnumerable<ToDo> ListToDisplayInNewWindow;

        public static async Task ActiveOrCreateNewWindow(int cate,bool createNewWindow)
        {
            var haskey = ViewIDs.ContainsKey(cate);
            if(!haskey && !createNewWindow)
            {
                return;
            }
            var newCoreAppView = CoreApplication.CreateNewView();
            var appView = ApplicationView.GetForCurrentView();
            
            await newCoreAppView.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async()=>
            {
                var window = Window.Current;
                var newAppView = ApplicationView.GetForCurrentView();

                newAppView.SetPreferredMinSize(new Size(300, 500));
                newAppView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                
                var frame = new Frame();
                window.Content = frame;
                frame.Navigate(typeof(CardViewPage),new MultiWindowsData(cate, ListToDisplayInNewWindow));
                window.Activate();

                int currentID = 0;
                if (!haskey)
                {
                    ViewIDs.Add(cate, newAppView.Id);
                    currentID = newAppView.Id;
                }
                else if (!haskey && !createNewWindow)
                {
                    return;
                }
                else if(haskey) currentID = ViewIDs[cate];

                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(currentID, ViewSizePreference.Default , appView.Id, ViewSizePreference.Default);
            });
        }

        
    }
}
