using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using JP.Utils.Helper;

namespace MyerList.Helper
{
    public static class JumpListHelper
    {

        public static async Task SetupJumpList()
        {
            if (!DeviceHelper.IsTH2OS) return;

            JumpList currentJumpList =await JumpList.LoadCurrentAsync();
            currentJumpList.Items.Clear();
            //var addItem = JumpListItem.CreateWithArguments(AddToken, "Add new memo");
            //addItem.Logo = new Uri("ms-appx:///Assets/cate_add.png");
            //currentJumpList.Items.Add(addItem);
            await currentJumpList.SaveAsync();
        }
    }
}
