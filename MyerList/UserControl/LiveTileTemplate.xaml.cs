using GalaSoft.MvvmLight.Messaging;
using HttpReqModule;
using JP.Utils.Data;
using JP.Utils.Debug;
using MyerList.Helper;
using MyerList.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace MyerList.UC
{
    public sealed partial class LiveTileTemplate : UserControl
    {

        public LiveTileTemplate()
        {
            this.InitializeComponent();

            Messenger.Default.Register<GenericMessage<ObservableCollection<ToDo>>>(this, MessengerTokens.UpdateTile, async msg =>
            {
                try
                {
                    if (LocalSettingHelper.GetValue("EnableTile") == "false")
                    {
                        UpdateTileHelper.ClearAllSchedules();
                        return;
                    }

                    var list = msg.Content;
                    //var allLists = from e in list group list by e.Category;

                    //if (allLists == null ) return;
                    //var firstList = allLists.FirstOrDefault().FirstOrDefault();
                    //List<ToDo> newList = new List<ToDo>();
                    //foreach (var item in firstList)
                    //{
                    //    var newToDo = new ToDo();
                    //    newToDo.IsDone = item.IsDone;
                    //    newToDo.Content = item.Content;
                    //    newList.Add(newToDo);
                    //}
                    //MultiWindowsHelper.ListToDisplayInNewWindow = newList;
                    //await MultiWindowsHelper.ActiveOrCreateNewWindow(allLists.FirstOrDefault().Key, false);

                    await UpdateCustomeTile(list);
                }
                catch(Exception)
                {

                }
                
            });
        }

        private void CleanUpTileTemplate()
        {
            LargeText0.Text = LargeText1.Text = LargeText2.Text = LargeText3.Text = "";
            WideText0.Text = WideText1.Text = WideText2.Text = WideText3.Text = "";
            MiddleText0.Text = MiddleText1.Text = MiddleText2.Text = MiddleText3.Text = "";

            LargeCount.Text = WideCount.Text = MiddleCount.Text = "";
        }

        public async Task UpdateCustomeTile(ObservableCollection<ToDo> schedules)
        {
            try
            {
                //关闭了磁贴更新
                if (LocalSettingHelper.GetValue("EnableTile") == "false")
                {
                    UpdateTileHelper.ClearAllSchedules();
                    return;
                }

                CleanUpTileTemplate();

                //透明磁贴
                if (LocalSettingHelper.GetValue("TransparentTile") == "true")
                {
                    LargeBackGrd.Background = WideBackGrd.Background = MiddleBackGrd.Background = SmallBackGrd.Background = new SolidColorBrush(Colors.Transparent);
                }


                List<string> undoList = new List<string>();

                foreach (var sche in schedules)
                {
                    if (!sche.IsDone)
                    {
                        undoList.Add(sche.Content);
                    }
                }


                //var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                //XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
                //badgeElement.SetAttribute("value", undoList.Count.ToString());
                //BadgeNotification badge = new BadgeNotification(badgeXml);
                //BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

                LargeText0.Text = WideText0.Text = MiddleText0.Text = undoList.ElementAtOrDefault(0) ?? "";
                LargeText1.Text = WideText1.Text = MiddleText1.Text = undoList.ElementAtOrDefault(1) ?? "";
                LargeText2.Text = WideText2.Text = MiddleText2.Text = undoList.ElementAtOrDefault(2) ?? "";
                LargeText3.Text = WideText3.Text = MiddleText3.Text = undoList.ElementAtOrDefault(3) ?? "";

                LargeText4.Text = undoList.ElementAtOrDefault(4) ?? "";
                LargeText5.Text = undoList.ElementAtOrDefault(4) ?? "";
                LargeText6.Text = undoList.ElementAtOrDefault(4) ?? "";
                LargeText7.Text = undoList.ElementAtOrDefault(4) ?? "";
                LargeText8.Text = undoList.ElementAtOrDefault(4) ?? "";

                LargeCount.Text = WideCount.Text = MiddleCount.Text = SmallCount.Text = undoList.Count.ToString();

                if (undoList.Count == 0)
                {
                    LargeText0.Text = WideText0.Text = MiddleText0.Text = "Enjoy your day ;-)";
                }

                UpdateTileHelper.ClearAllSchedules();

                //少于4个待办事项，不轮播
                if (undoList.Count <= 4)
                {
                    await UpdateTileHelper.UpdatePersonalTile(LargeGrid, WideGrid, MiddleGrid, SmallGrid,true, false);
                }
                else
                {
                    //把前4条插入轮播
                    await UpdateTileHelper.UpdatePersonalTile(LargeGrid, WideGrid, MiddleGrid, SmallGrid,true, true);

                    if (undoList.Count > 4)
                    {
                        WideText0.Text = MiddleText0.Text = undoList.ElementAtOrDefault(4) ?? "";
                        WideText1.Text = MiddleText1.Text = undoList.ElementAtOrDefault(5) ?? "";
                        WideText2.Text = MiddleText2.Text = undoList.ElementAtOrDefault(6) ?? "";
                        WideText3.Text = MiddleText3.Text = undoList.ElementAtOrDefault(7) ?? "";

                        //把5~8条加入轮播
                        await UpdateTileHelper.UpdatePersonalTile(LargeGrid, WideGrid, MiddleGrid, SmallGrid,false, true);
                    }
                    if (undoList.Count > 8)
                    {
                        WideText0.Text = MiddleText0.Text = undoList.ElementAtOrDefault(8) ?? "";
                        WideText1.Text = MiddleText1.Text = undoList.ElementAtOrDefault(9) ?? "";
                        WideText2.Text = MiddleText2.Text = undoList.ElementAtOrDefault(10) ?? "";
                        WideText3.Text = MiddleText3.Text = undoList.ElementAtOrDefault(11) ?? "";

                        //大于8的加入轮播
                        await UpdateTileHelper.UpdatePersonalTile(LargeGrid, WideGrid, MiddleGrid, SmallGrid, false,true);
                    }
                }

            }
            catch (Exception e)
            {
                var task = ExceptionHelper.WriteRecord(e);

            }

        }

    }
}
