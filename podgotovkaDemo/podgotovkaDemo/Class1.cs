using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace podgotovkaDemo
{
    internal static class Class1
    {
        public static RenderTargetBitmap ToImage(this UIElement element)
        {
            var size = new Size(element.RenderSize.Width, element.RenderSize.Height);
            var renderTargetBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(element), null, new Rect(new Point(), size));
            }

            renderTargetBitmap.Render(visual);
            return renderTargetBitmap;
        }
    }
}
