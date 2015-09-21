using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using Windows.UI;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using NotificationsExtensions.TileContent;


// ReSharper disable All

namespace HttpReqModule
{
    public static class UpdateTileHelper
    {
        /// <summary>
        /// 支持宽/中/小磁贴的更新
        /// </summary>
        /// <param name="wideTileGrid"></param>
        /// <param name="mediumTileGrid"></param>
        /// <param name="smallTileGrid"></param>
        /// <returns></returns>
        public async static Task<bool> UpdatePersonalTile(UIElement largeTileGrid, UIElement wideTileGrid, UIElement mediumTileGrid, UIElement smallTileGrid,bool updateLarge, bool isAddToSchedule = false)
        {
            try
            {
                string largeName = await SaveUIElementToFile(largeTileGrid, TileCategory.Large);
                string wideName=await SaveUIElementToFile(wideTileGrid, TileCategory.Wide);
                string middleName = await SaveUIElementToFile(mediumTileGrid, TileCategory.Medium);
                string smallName = await SaveUIElementToFile(smallTileGrid, TileCategory.Small);

                if(string.IsNullOrEmpty(wideName) || string.IsNullOrEmpty(middleName) || string.IsNullOrEmpty(smallName))
                {
                   throw new NullReferenceException();
                }
                //small
                var smallTileContent = TileContentFactory.CreateTileSquare71x71Image();
                smallTileContent.Image.Src = "ms-appdata:///local/" + smallName;

                //medium
                var mediumTileContent = TileContentFactory.CreateTileSquare150x150Image();
                mediumTileContent.RequireSquare71x71Content = true;
                mediumTileContent.Square71x71Content = smallTileContent;
                mediumTileContent.Image.Src = "ms-appdata:///local/" + middleName;
                mediumTileContent.Branding = TileBranding.Logo;

                //wide
                var wideTileContent = TileContentFactory.CreateTileWide310x150Image();
                wideTileContent.RequireSquare150x150Content = true;
                wideTileContent.Square150x150Content = mediumTileContent;
                wideTileContent.Image.Src = "ms-appdata:///local/" + wideName;
                wideTileContent.Branding = TileBranding.Logo;

                var largeTileContent = TileContentFactory.CreateTileSquare310x310Image();
                largeTileContent.RequireWide310x150Content = true;
                largeTileContent.Wide310x150Content = wideTileContent;
                largeTileContent.Image.Src = "ms-appdata:///local/" + largeName;
                largeTileContent.Branding = TileBranding.Logo;

                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(isAddToSchedule);

                var notification = updateLarge ? largeTileContent.CreateNotification() : wideTileContent.CreateNotification();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        //NO USE
        public static void UpdateNormalTile(List<string> undoList, bool isAddToSchedule = false)
        {
            var mediumContent = TileContentFactory.CreateTileSquare150x150Text01();
            mediumContent.TextBody1.Text = undoList.Count == 0 ? "Enjoy your day ;-)" : undoList.ElementAtOrDefault(0);
            mediumContent.TextBody2.Text = undoList.ElementAtOrDefault(1);
            mediumContent.TextBody3.Text = undoList.ElementAtOrDefault(2);
            mediumContent.TextHeading.Text = "TO-DO: " + undoList.Count.ToString();

            var wideContent = TileContentFactory.CreateTileWide310x150BlockAndText01();
            wideContent.RequireSquare150x150Content = true;
            wideContent.Square150x150Content = mediumContent;
            wideContent.TextBlock.Text = "  " + undoList.Count.ToString();
            wideContent.TextSubBlock.Text = "To-do";
            wideContent.TextBody1.Text = undoList.Count == 0 ? "Enjoy your day" : undoList.ElementAtOrDefault(0);
            wideContent.TextBody2.Text = undoList.ElementAtOrDefault(1);
            wideContent.TextBody3.Text = undoList.ElementAtOrDefault(2);
            wideContent.TextBody4.Text = undoList.ElementAtOrDefault(3);
            wideContent.Branding = TileBranding.Logo;

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(isAddToSchedule);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(wideContent.CreateNotification());

            if(undoList.Count>3 && isAddToSchedule)
            {
                wideContent.TextBody1.Text = undoList.ElementAtOrDefault(4);
                wideContent.TextBody2.Text = undoList.ElementAtOrDefault(5);
                wideContent.TextBody3.Text = undoList.ElementAtOrDefault(6);
                wideContent.TextBody4.Text = undoList.ElementAtOrDefault(7);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(wideContent.CreateNotification());

                if(undoList.Count>6)
                {
                    wideContent.TextBody1.Text = undoList.ElementAtOrDefault(8);
                    wideContent.TextBody2.Text = undoList.ElementAtOrDefault(9);
                    wideContent.TextBody3.Text = undoList.ElementAtOrDefault(10);
                    wideContent.TextBody4.Text = undoList.ElementAtOrDefault(11);
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(wideContent.CreateNotification());
                }
            }
        }

        /// <summary>
        /// 保持UIElement 元素到文件
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="cate">磁贴种类</param>
        /// <returns></returns>
        private async static Task<string> SaveUIElementToFile(UIElement element,TileCategory cate)
        {
            try
            {
                string filename = "";
                switch (cate)
                {
                    case TileCategory.Small: filename = "smallTile.png"; break;
                    case TileCategory.Medium: filename = "mediumTile.png"; break;
                    case TileCategory.Wide: filename = "wideTile.png"; break;
                    case TileCategory.Large: filename = "largeTile.png"; break;
                    default: filename = "largeTile.png"; break;
                }

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                CachedFileManager.DeferUpdates(file);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var bitmap = new RenderTargetBitmap();
                    await bitmap.RenderAsync(element);
                    var pixels = await bitmap.GetPixelsAsync();

                    var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixels.ToArray());

                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(file);

                return file.Name;
            }
            catch(Exception)
            {
                
                return null;
            }
            
        }

        public static void ClearAllSchedules()
        {
            var notiList = TileUpdateManager.CreateTileUpdaterForApplication().GetScheduledTileNotifications();
            foreach (var noti in notiList)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().RemoveFromSchedule(noti);
            }
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }
    }

    public enum TileCategory
    {
        Small,
        Medium,
        Wide,
        Large
    }
}
