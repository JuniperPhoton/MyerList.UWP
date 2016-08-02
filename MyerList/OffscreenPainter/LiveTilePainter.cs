using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using MyerList.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;

namespace MyerList.OffscreenPainter
{
    public class LiveTilePainter
    {
        public IReadOnlyList<ToDo> List { get; set; }

        public LiveTileKind TileKind { get; set; }

        public LiveTilePainter()
        {

        }

        public async Task<StorageFile> DrawAsync()
        {
            if (!CheckResources()) throw new ArgumentOutOfRangeException();

            var size = GetTileSize();
            var device = CanvasDevice.GetSharedDevice();
            var offscreen = new CanvasRenderTarget(device, (float)size.Width, (float)size.Height, 96);

            using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                for(int i=0;i<List.Count();i++)
                {
                    var todo = List[i];
                    ds.DrawTextLayout(CreateTextLayout(device, todo.Content), new Vector2(10f, 10 * i), 
                        new CanvasSolidColorBrush(device,Colors.White));
                }
            }

            var cacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("LiveTile", CreationCollisionOption.OpenIfExists);
            var file = await cacheFolder.CreateFileAsync($"{TileKind.ToString()}.png", CreationCollisionOption.GenerateUniqueName);

            var bytes = offscreen.GetPixelBytes();
            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fs);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)size.Width, (uint)size.Height, 96, 96, bytes);
                await encoder.FlushAsync();
            }

            return file;
        }

        private bool CheckResources()
        {
            if (List != null)
            {
                if(TileKind==LiveTileKind.Medium || TileKind == LiveTileKind.Wide)
                {
                    if (List.Count() <= 4) return true;
                    else return false;
                }
                else
                {
                    if (List.Count() <= 8) return true;
                    else return false;
                }
            }
            else return false;
        }

        private Size GetTileSize()
        {
            switch (TileKind)
            {
                case LiveTileKind.Medium:
                    {
                        return new Size(150, 150);
                    };
                case LiveTileKind.Wide:
                    {
                        return new Size(300, 150);
                    };
                case LiveTileKind.Large:
                    {
                        return new Size(300, 300);
                    };
                default:
                    {
                        return new Size(0, 0);
                    };
            }
        }

        private CanvasTextLayout CreateTextLayout(ICanvasResourceCreator canvas, string text)
        {
            var textLayout = new CanvasTextLayout(canvas, text, new CanvasTextFormat()
            {
                FontSize = 25,
            }, 10000f, 10000f);
            return textLayout;
        }
    }
}
