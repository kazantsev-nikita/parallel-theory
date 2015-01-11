using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace lr2.ImageBlender
{

    public static class BitmapExtensions
    {
        public static void CopyPixels(this Bitmap source, Bitmap destination)
        {
            try { 
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    var p = source.GetPixel(x, y);
                    destination.SetPixel(x, y, Color.FromArgb(p.R, p.G, p.B));
                }
            }
                }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory();
                File.AppendAllText(Path.Combine(path, "Exception.txt"), ex.Message + ex.StackTrace + Environment.NewLine);
            }
        }

        public static void setNewColour(this Bitmap layer)
        {
            try
            {
                for (int y = 0; y < layer.Height; y++)
                {
                    for (int x = 0; x < layer.Width; x++)
                    {
                        var pixel = layer.GetPixel(x, y);
                        int colour = (int)(pixel.R * 0.4 + pixel.G * 0.49 + pixel.B * 0.44);
                        layer.SetPixel(x, y, Color.FromArgb(colour, colour, colour));
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory();
                File.AppendAllText(Path.Combine(path, "Exception.txt"), ex.Message + ex.StackTrace + Environment.NewLine);
            }
        }
    }
}
