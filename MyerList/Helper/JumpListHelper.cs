using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using System.Reflection;
using Windows.System.Profile;
using JP.Utils.Device;
using JP.Utils.Helper;

namespace MyerList.Helper
{
    public static class JumpListHelper
    {
        public const string AddToken = "Add";
        public const string DefaultToken = "Default";
        public const string WorkToken = "Work";
        public const string LifeToken = "Life";
        public const string FamilyToken = "Family";
        public const string EnterToken = "Enter";
        public static async Task SetupJumpList()
        {
            if (!DeviceHelper.IsTH2OS) return;

            JumpList currentJumpList =await JumpList.LoadCurrentAsync();
            currentJumpList.Items.Clear();
            var addItem = JumpListItem.CreateWithArguments(AddToken, "Add new memo");
            addItem.Logo = new Uri("ms-appx:///Assets/cate_add.png");
            //var itemSep = JumpListItem.CreateSeparator();
            //var item2 = JumpListItem.CreateWithArguments(AddToken, "All");
            //item2.Logo = new Uri("ms-appx:///Assets/cate_default.png");
            //var item3 = JumpListItem.CreateWithArguments(WorkToken, "Work");
            //item3.Logo = new Uri("ms-appx:///Assets/cate_work.png");
            //var item4 = JumpListItem.CreateWithArguments(LifeToken, "Life");
            //item4.Logo = new Uri("ms-appx:///Assets/cate_life.png");
            //var item5 = JumpListItem.CreateWithArguments(FamilyToken, "Family");
            //item5.Logo = new Uri("ms-appx:///Assets/cate_family.png");
            //var item6 = JumpListItem.CreateWithArguments(EnterToken, "Enter");
            //item6.Logo = new Uri("ms-appx:///Assets/cate_enter.png");
            currentJumpList.Items.Add(addItem);
            //currentJumpList.Items.Add(itemSep);
            //currentJumpList.Items.Add(item2);
            //currentJumpList.Items.Add(item3);
            //currentJumpList.Items.Add(item4);
            //currentJumpList.Items.Add(item5);
            //currentJumpList.Items.Add(item6);
            await currentJumpList.SaveAsync();
        }
    }
}
