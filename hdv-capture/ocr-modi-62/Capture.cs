using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ocr_modi
{
    public class Hdv62
    {
        [DllImport("Hdvs.dll", EntryPoint = "Hdv62_GetErrorText")]
        public static extern int GetErrorText(int code, StringBuilder Text);
        public static string GetErrorText(int code)
        {
            StringBuilder buf = new StringBuilder(250, 250);
            if (GetErrorText(code, buf) == 0)
                return buf.ToString();
            return null;
        }

        public delegate void HDV62CALLBACK(uint Number, uint EventType);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_DeviceOpen")]
        public static extern int DeviceOpen(uint Number);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_DeviceClose")]
        public static extern int DeviceClose(uint Number);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetCallbackSelector")]
        public static extern int SetCallbackSelector(uint Number, uint Mode);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetCallback")]
        public static extern int SetCallback(uint Number, HDV62CALLBACK Fun);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetAcquisitionFrameCount")]
        public static extern int SetAcquisitionFrameCount(uint Number, uint Count);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_AcquisitionStart")]
        public static extern int AcquisitionStart(uint Number);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_AcquisitionStop")]
        public static extern int AcquisitionStop(uint Number);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_GetImageStream")]
        public static extern int GetImageStream(uint Number, out IntPtr Buffer);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_GetSensorStatus")]
        public static extern int GetSensorStatus(uint Number, out uint Locked);
        
        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_OneShot")]
        public static extern int OneShot(uint Number, uint Timeout);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SaveImage")]
        public static extern int SaveImage(uint Number, string FileName);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_GetImageOrientation")]
        public static extern int GetImageOrientation(uint Number, out uint Value);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetChannel")]
        public static extern int SetChannel(uint Number, uint Channel);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetSensorFormat")]
        public static extern int SetSensorFormat(uint Number, uint Format);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetOutputFormat")]
        public static extern int SetOutputFormat(uint Number, uint Format);

        [DllImport("Hdv62.dll", EntryPoint = "Hdv62_SetImageOrientation")]
        public static extern int SetImageOrientation(uint Number, uint Value);

    }

    public static class Capture
    {
        private const int DEVICESNUMBER = 0;
        private const int WIDTH = 1920;
        private const int HEIGHT = 1080;
        private const int BUFSIZE = WIDTH * HEIGHT * 3;

        private static Hdv62.HDV62CALLBACK cb;
        public static IntPtr d_Buffer;
        public static IntPtr m_Buffer;
        public static String Status = "on";
        
        public static int fps = 0;
        private static int lastcount = 0;
        public volatile static int capcount = 0;
        private static System.Threading.Timer t_C;

        private static void TimerCall(object state)
        {
            uint locked = 0;
            int hr = Hdv62.GetSensorStatus(DEVICESNUMBER, out locked);
            if (hr >= 0 && locked == 1)
            {
                Status = "on";
            }
            else
            {
                Status = "off";
            }
            fps = capcount - lastcount;
            lastcount = capcount;
        }
        
        public static void Open()
        {
            d_Buffer = Marshal.AllocHGlobal(BUFSIZE);
            cb = new Hdv62.HDV62CALLBACK(CatchedData);
            t_C = new System.Threading.Timer(TimerCall, null, 0, 1000);
            Hdv62.DeviceOpen(DEVICESNUMBER);

            Hdv62.SetChannel(0, 2);
            Hdv62.SetSensorFormat(0, 7);
            Hdv62.SetOutputFormat(0, 0);
            Hdv62.SetImageOrientation(0, 0);
            Hdv62.SetCallbackSelector(DEVICESNUMBER, 0);
            Hdv62.SetCallback(DEVICESNUMBER, cb);
            Hdv62.SetAcquisitionFrameCount(DEVICESNUMBER, 0);
            Hdv62.AcquisitionStart(DEVICESNUMBER);
        }
        
        public static void Close()
        {
            Hdv62.AcquisitionStop(DEVICESNUMBER);
            Hdv62.SetCallbackSelector(DEVICESNUMBER, 0);
            Hdv62.SetCallback(DEVICESNUMBER, null);
            Hdv62.DeviceClose(DEVICESNUMBER);
        }
        
        public static Bitmap GetPicture()
        {
            IntPtr t_Buffer = new IntPtr();
            Interlocked.Exchange(ref t_Buffer, m_Buffer);
            if (t_Buffer != IntPtr.Zero)
            {
                CsConvert.RECT rct = new CsConvert.RECT();
                rct.left = 0; rct.right = WIDTH; rct.top = 0; rct.bottom = HEIGHT;
                uint m_ImageOrg = 0;
                Hdv62.GetImageOrientation(DEVICESNUMBER, out m_ImageOrg);
                CsConvert.Rgb24ToRgb24(WIDTH, HEIGHT, d_Buffer, t_Buffer, rct, m_ImageOrg);


                Bitmap f_Bmp = new Bitmap(WIDTH, HEIGHT, ((WIDTH * 3) + 3) & ~3,
                                            PixelFormat.Format24bppRgb, d_Buffer);
                return f_Bmp;
            }
            return null;
        }

        public static bool IsColorBar(Bitmap source)
        {
            int[] color_seq = {
                0x00F8F8F8, 0x00F8F800, 0x0000F8F8, 0x0000F800,//white(255,255,255) yellow(255,255,0) cyan(0,255,255) green(0,255,0)
                0x00F800F8, 0x00F80000, 0x000000F8, 0x00000000,//purple(255,0,255) red(255,0,0) blue(0,0,255) black(0,0,0)
            };
            bool result = true;
            for (int i = 1; i <= 16; i += 2)
            {
                Color pixelColor = source.GetPixel(i * source.Width / 16, source.Height / 16);
                if ((pixelColor.ToArgb() & 0x00F8F8F8) != color_seq[i / 2])
                {
                    Console.WriteLine(string.Format("index:{0} color:{1:6X}", i / 2, pixelColor.ToArgb() & 0x00F8F8F8));
                    result = false;
                }
            }
            return result;
        }

        public static Bitmap ResizePicture(Bitmap fBmp, Size tSize)
        {
            if (fBmp != null)
            {
                Bitmap m_Bmp = new Bitmap(tSize.Width, tSize.Height);
                Graphics g = Graphics.FromImage(m_Bmp);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(fBmp, new Rectangle(0, 0, tSize.Width, tSize.Height),
                    new Rectangle(0, 0, fBmp.Width, fBmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return m_Bmp;
            }
            return null;
        }

        public static void CatchedData(uint Number, uint EventType)
        {
            if (EventType == 0)
            {
                capcount++;
                IntPtr tempbuf = new IntPtr();
                if (Hdv62.GetImageStream(Number, out tempbuf) == 0)
                {
                    Interlocked.Exchange(ref m_Buffer, tempbuf);
                }
            }
        }
    }

    public class CsConvert
    {
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        private CsConvert()
        {
            // Prevent people from trying to instantiate this class
        }
        public unsafe static void Rgb24ToRgb24(int Width, int Height, IntPtr Dest, IntPtr Src, RECT Cropping, uint Orientation)
        {
            int DestWidth = Cropping.right - Cropping.left;
            int DestHeight = Cropping.bottom - Cropping.top;

            int SrcLine = Width * 3;
            int DestLine = ((DestWidth * 3) + 3) & ~3;  // align to 4

            int SrcOffset;

            if (Orientation == 0)
            {
                SrcOffset = (Height - Cropping.top - 1) * SrcLine
                           + Cropping.left * 3;
            }
            else
            {
                SrcOffset = Cropping.top * SrcLine
                           + Cropping.left * 3;
                SrcLine = -SrcLine;
            }

            byte* src = (byte*)Src.ToPointer() + SrcOffset;
            byte* dest = (byte*)Dest.ToPointer();
            byte* s;
            byte* d;

            for (int j = 0; j < DestHeight; j++)
            {
                s = src;
                d = dest;

                for (int i = 0; i < DestWidth; i++)
                {
                    *d++ = *s++;    // B
                    *d++ = *s++;    // G
                    *d++ = *s++;    // R
                }

                src -= SrcLine;
                dest += DestLine;
            }
        }

    }
}
