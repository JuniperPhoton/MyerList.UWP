using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace MyerList.Helper
{
    public enum DeviceKind
    {
        Windows,
        WindowsPhone
    }
    public class BackgroundTaskHelper
    {
        public static void UnRegisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "TileUpdaterTask")
                {
                    task.Value.Unregister(true);
                }
            }
        }

        public static async void RegisterBackgroundTask()
        {
            try
            {
                BackgroundAccessStatus status = BackgroundAccessStatus.Unspecified;

                status = await BackgroundExecutionManager.RequestAccessAsync();

                if (status == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                    status == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
                {
                    UnRegisterBackgroundTask();

                    BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                    taskBuilder.Name = "TileUpdaterTask";
                    taskBuilder.TaskEntryPoint = "BackgroundTasks.TileUpdaterTask";
                    taskBuilder.SetTrigger(new TimeTrigger(15, false));
                    var registration = taskBuilder.Register();
                }
            }
            catch (Exception)
            {
                
            }
        }

    }
}
