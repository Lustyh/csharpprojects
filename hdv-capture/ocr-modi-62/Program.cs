using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ocr_modi
{
    class Program
    {
        static void Main(string[] args)
        {
            //disable multi process runing
            bool ret = false;
            Mutex running = new Mutex(true, Assembly.GetExecutingAssembly().FullName, out ret);
            if (!ret)
            {
                Console.Error.WriteLine("another thread was runing!");
                Thread.Sleep(100);
                return;
            }
            running.ReleaseMutex();
            //check color bar
            if (args.Length != 1)
            {
                Console.Error.WriteLine("ocr-modi.exe [path]");
                return;
            }
            string filepath = args[0].Replace("\"", "");
            try
            {
                Capture.Open();
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(1000);
                    if (!(Capture.Status.Equals("on") && Capture.fps > 10))
                    {
                        Console.WriteLine("No video signal!");
                        continue;
                    }
                    Bitmap pic = Capture.GetPicture();
                    if (pic == null) continue;
                    try
                    {
                        pic.Save(filepath);
                        Recognition(filepath);
                    }catch(Exception x)
                    {
                        Console.WriteLine(x.Message);
                    }
                    break;
                    /*
                    if (isAvailableImage(pic))
                    {
                        //Bitmap tPic = Capture.ResizePicture(pic, new Size(480, 270));
                        //tPic.Save(args[0].Replace("\"", ""));
                        pic.Save(filepath);
                        Recognition(filepath);
                        //Console.WriteLine("PASS");
                        //tPic.Dispose();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Not a bootup screen, retry" + (i + 1).ToString());
                        string fail = filepath + $".FAIL-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                        pic.Save(fail);
                        Recognition(fail);
                        pic.Dispose();
                    }
                    */
                }
                Capture.Close();
            }
            catch (Exception x)
            {
                try { Capture.Close(); } catch (Exception) { }
                Console.WriteLine(x.Message);
            }
            System.Environment.Exit(0);
        }

        static bool isAvailableImage(Bitmap sources)
        {
            //Rectangle rect = new Rectangle(100, 100, 55, 55);
            Rectangle rect = new Rectangle(190, 210, 55, 55);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(sources, 0, 0, rect, GraphicsUnit.Pixel);
            int count = 0;
            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    Color pix = bmp.GetPixel(x, y);
                    //if (pix.R >= 50 || pix.G >= 50 || pix.B >= 50) count++;
                    if (pix.R >= 100 || pix.G >= 100 || pix.B >= 100) count++;
                }
            }
            return count > 800;
        }

        static bool Recognition(string imageFile)
        {
            string txt;
            var langs = MODI.MiLANGUAGES.miLANG_ENGLISH;
            // MODI.MiLANGUAGES.miLANG_CHINESE_SIMPLIFIED; 中文含英文
            // MODI.MiLANGUAGES.miLANG_JAPANESE; 日文含英文

            var doc = new MODI.Document();
            var image = default(MODI.Image);
            var layout = default(MODI.Layout);
            var sb = new StringBuilder();
            try
            {
                doc.Create(imageFile);
                doc.OCR(langs, true, true);

                for (int i = 0; i < doc.Images.Count; i++)
                {
                    image = (MODI.Image)doc.Images[i];
                    layout = image.Layout;
                    sb.AppendLine(string.Format("{0}, {1}", i, layout.Text));
                }
                doc.Close(false);
                txt = Regex.Replace(sb.ToString(), "[\r|\n]", "");
                Console.Write(txt);
                Console.WriteLine();
                return true;
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                doc = null;
                image = null;
                layout = null;
                return false;
            }
        }
    }
}
