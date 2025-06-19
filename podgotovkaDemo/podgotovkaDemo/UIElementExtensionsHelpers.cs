using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.IdentityModel.Protocols;
using podgotovkaDemo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Word = Microsoft.Office.Interop.Word;

internal static class UIElementExtensionsHelpers
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