using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using NotificationsExtensions.TileContent;
using JP.Utils.Helper;
using JP.Utils.Debug;


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
                if (DeviceHelper.IsMobile) updateLarge = false;

                var largeFile = await SaveUIElementToFile(largeTileGrid, TileCategory.Large);
                var wideFile=await SaveUIElementToFile(wideTileGrid, TileCategory.Wide);
                var mediumFile = await SaveUIElementToFile(mediumTileGrid, TileCategory.Medium);
                //var smallFile = await SaveUIElementToFile(smallTileGrid, TileCategory.Small);
                
                if(wideFile==null || mediumFile==null)
                {
                   throw new NullReferenceException();
                }
                //small
                //var smallTileContent = TileContentFactory.CreateTileSquare71x71Image();
                //smallTileContent.Image.Src = smallFile.Path;

                //medium
                var mediumTileContent = TileContentFactory.CreateTileSquare150x150Image();
                mediumTileContent.RequireSquare71x71Content = false;
                //mediumTileContent.Square71x71Content = smallTileContent;
                mediumTileContent.Image.Src = mediumFile.Path;
                mediumTileContent.Branding = TileBranding.Logo;

                //wide
                var wideTileContent = TileContentFactory.CreateTileWide310x150Image();
                wideTileContent.RequireSquare150x150Content = true;
                wideTileContent.Square150x150Content = mediumTileContent;
                wideTileContent.Image.Src = wideFile.Path;
                wideTileContent.Branding = TileBranding.Logo;

                var largeTileContent = TileContentFactory.CreateTileSquare310x310Image();
                largeTileContent.RequireWide310x150Content = true;
                largeTileContent.Wide310x150Content = wideTileContent;
                largeTileContent.Image.Src = largeFile.Path;
                largeTileContent.Branding = TileBranding.Logo;

                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(isAddToSchedule);

                var notification = updateLarge ? largeTileContent.CreateNotification() : wideTileContent.CreateNotification();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

                return true;
            }
            catch(Exception e)
            {
                var task = ExceptionHelper.WriteRecordAsync(e, nameof(UpdateTileHelper), nameof(UpdatePersonalTile));
                return false;
            }
        }
 
        /// <summary>
        /// 保持UIElement 元素到文件
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="cate">磁贴种类</param>
        /// <returns></returns>
        private async static Task<StorageFile> SaveUIElementToFile(UIElement element,TileCategory cate)
        {
            try
            {
                string filename;
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

                var bitmap = new RenderTargetBitmap();
                await bitmap.RenderAsync(element);
                var pixels = await bitmap.GetPixelsAsync();
                var logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixels.ToArray());

                    await encoder.FlushAsync();
                }
                await CachedFileManager.CompleteUpdatesAsync(file);

                return file;
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
