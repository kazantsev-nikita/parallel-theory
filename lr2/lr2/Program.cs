using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Globalization;
using lr2.ImageBlender;

namespace lr2
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string sourceDir = Directory.GetCurrentDirectory();
            string image = "1.JPG";            
            string destDir = Directory.GetCurrentDirectory();

            if (args.Length > 0) sourceDir = args[0];
            if (args.Length > 1) image = args[1];
            if (args.Length > 2) destDir = args[2];

            string path = Path.Combine(sourceDir, image);

            CheckDirectoryExists(sourceDir);
            CheckFileExists(path);
            CheckDirectoryExists(destDir);

            Bitmap source = new Bitmap(path);
            Bitmap layer = new Bitmap(source.Width, source.Height);

            using (Bitmap result = layer) { 
                ParallelImageProcessing(source, layer);
                result.Save(Path.Combine(destDir, "Blended.jpg"));
            }
        }

        private static void CheckDirectoryExists(string dirName)
        {
            if (!Directory.Exists(dirName))
            {
                Console.WriteLine("Directory does not exist: {0}", dirName);
                Environment.Exit(0);
            }
        }

        private static void CheckFileExists(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist: {0}", path);
                Environment.Exit(0);
            }
        }           

        private static void ParallelImageProcessing(Bitmap source, Bitmap layer)
        {
            try { 
                source.CopyPixels(layer);
                Parallel.Invoke(
                    () => ChangeColour(layer, 0, 0, source.Width, source.Height));
                }
            catch (Exception ex)
            {
                string path = Directory.GetCurrentDirectory();
                File.AppendAllText(Path.Combine(path, "Exception.txt"), ex.Message + ex.StackTrace + Environment.NewLine);
            }
        }

        private static void ChangeColour(Bitmap layer, int xMin, int yMin, int xMax, int yMax)
        {
            try { 
                if (xMax < 500 || yMax < 500) 
                { 
                    layer.setNewColour();
                    return;
                }
                else
                {
                    int xMid = xMin + (xMax - xMin) / 2;
                    int yMid = yMin + (yMax - yMin) / 2;
                
                    Action[] actions = new Action[4];
                    actions[0] = (() => ChangeColour(layer, xMin, yMin, xMid, yMid));
                    actions[1] = (() => ChangeColour(layer, xMid, yMin, xMax, yMid));
                    actions[2] = (() => ChangeColour(layer, xMin, yMid, xMid, yMax));
                    actions[3] = (() => ChangeColour(layer, xMid, yMid, xMax, yMax));
                    
                    Parallel.Invoke(actions);
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
 

