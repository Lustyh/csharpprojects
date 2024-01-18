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
    public class Hdv72
    {
        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetErrorText")]
        public static extern int GetErrorText(int code, StringBuilder Text);
        public static string GetErrorText(int code)
        {
            StringBuilder buf = new StringBuilder(250, 250);
            if (GetErrorText(code, buf) == 0)
                return buf.ToString();
            return null;
        }

        public delegate void Hdv72CALLBACK(uint Number, uint EventType);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_DeviceOpen")]
        public static extern int DeviceOpen(uint Number);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_DeviceClose")]
        public static extern int DeviceClose(uint Number);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCallbackSelector")]
        public static extern int SetCallbackSelector(uint Number, uint Mode);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCallback")]
        public static extern int SetCallback(uint Number, Hdv72CALLBACK Fun);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAcquisitionFrameCount")]
        public static extern int SetAcquisitionFrameCount(uint Number, uint Count);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_AcquisitionStart")]
        public static extern int AcquisitionStart(uint Number);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_AcquisitionStop")]
        public static extern int AcquisitionStop(uint Number);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetImageStream")]
        public static extern int GetImageStream(uint Number, out IntPtr Buffer);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSensorStatus")]
        public static extern int GetSensorStatus(uint Number, out uint Locked);
        
        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_OneShot")]
        public static extern int OneShot(uint Number, uint Timeout);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SaveImage")]
        public static extern int SaveImage(uint Number, string FileName);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetImageOrientation")]
        public static extern int GetImageOrientation(uint Number, out uint Value);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetChannel")]
        public static extern int SetChannel(uint Number, uint Channel);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetSensorFormat")]
        public static extern int SetSensorFormat(uint Number, uint Format);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetOutputFormat")]
        public static extern int SetOutputFormat(uint Number, uint Format);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetImageOrientation")]
        public static extern int SetImageOrientation(uint Number, uint Value);

        //
        // CEC
        //

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCECPowerUp")]
        public static extern int SetCECPowerUp(uint Number, uint Power);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCECNACK")]
        public static extern int SetCECNACK(uint Number, uint SetCECNACK);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCECLogicalAddress")]
        public static extern int SetCECLogicalAddress(uint Number, uint BufferNumber, uint Logical_Address);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCECGlitchFilter")]
        public static extern int SetCECGlitchFilter(uint Number, uint Glitch_Filter);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_WriteCECCommand")]
        public static extern int WriteCECCommand(uint Number, byte[] Value, uint Length);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_ReadCECCommand")]
        public static extern int ReadCECCommand(uint Number, uint BufferNumber, byte[] Value, uint Length);

        //
        // EDID
        //
        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetEdid")]
        public static extern int SetEdid(uint Number, uint Port, ushort Offset, byte[] Data, ushort Length);

        [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetEdid")]
        public static extern int GetEdid(uint Number, uint Port, ushort Offset, byte[] pData, ushort Length);

    }

    public static class Capture
    {
        private const int DEVICESNUMBER = 0;
        /*
        private const int WIDTH = 3840;
        private const int HEIGHT = 2160;
        private const int FORMAT = 13;
        /*/

#if P1080
        public const int WIDTH = 1920;
        public const int HEIGHT = 1080;
        private const int FORMAT = 12;
#elif P720
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;
        private const int FORMAT = 5;
#endif

        private const int BUFSIZE = WIDTH * HEIGHT * 3;

        private static Hdv72.Hdv72CALLBACK cb;
        public static IntPtr d_Buffer;
        public static IntPtr m_Buffer;
        public static String Status = "on";
        
        public static int fps = 0;
        public static uint format = 12;
        private static int lastcount = 0;
        private volatile static int capcount = 0;
        private static System.Threading.Timer t_C;

        private static void TimerCall(object state)
        {
            uint locked = 0;
            int hr = Hdv72.GetSensorStatus(DEVICESNUMBER, out locked);
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
        
        public static int LoadEDID(byte[] data)
        {
            int hr = Hdv72.SetEdid(DEVICESNUMBER, 0, 0, data, (ushort)data.Length);
            return hr;
        }

        public static int EnableCEC()
        {
            int hr = 0;
            hr += Hdv72.SetCECPowerUp(DEVICESNUMBER, 1);
            hr += Hdv72.SetCECLogicalAddress(DEVICESNUMBER, 0, 0xf0);
            return hr;
        }

        public static int WriteCEC(byte[] data)
        {
            return Hdv72.WriteCECCommand(DEVICESNUMBER, data, (uint)data.Length);
        }

        public static byte[] ReadCEC()
        {
            byte[] data = new byte[16];
            int hr = Hdv72.ReadCECCommand(DEVICESNUMBER, 0, data, (uint)data.Length);
            if (hr < 0) return new byte[] { };
            return data;
        }
        
        public static int Open()
        {
            int hr = 0;
            //BUFSIZE = WIDTH * HEIGHT * 3;
            d_Buffer = Marshal.AllocHGlobal(BUFSIZE);
            cb = new Hdv72.Hdv72CALLBACK(CatchedData);
            t_C = new System.Threading.Timer(TimerCall, null, 0, 1000);
            hr += Hdv72.DeviceOpen(DEVICESNUMBER);

            hr += Hdv72.SetChannel(DEVICESNUMBER, 2);
            hr += Hdv72.SetSensorFormat(DEVICESNUMBER, format);
            hr += Hdv72.SetOutputFormat(DEVICESNUMBER, 0);
            hr += Hdv72.SetImageOrientation(DEVICESNUMBER, 0);
            hr += Hdv72.AcquisitionStart(DEVICESNUMBER);
            hr += Hdv72.SetCallbackSelector(DEVICESNUMBER, 0);
            hr += Hdv72.SetCallback(DEVICESNUMBER, cb);
            hr += Hdv72.SetAcquisitionFrameCount(DEVICESNUMBER, 0);
            hr += Hdv72.AcquisitionStart(DEVICESNUMBER);
            return hr;
        }
        
        public static int Close()
        {
            int hr = 0;
            hr += Hdv72.SetCallbackSelector(DEVICESNUMBER, 0);
            hr += Hdv72.SetCallback(DEVICESNUMBER, null);
            hr += Hdv72.AcquisitionStop(DEVICESNUMBER);
            hr += Hdv72.DeviceClose(DEVICESNUMBER);
            if (d_Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(d_Buffer);
            }
            return hr;
        }
        

        public static Bitmap GetPicture()
        {
            IntPtr t_Buffer = new IntPtr();
            Interlocked.Exchange(ref t_Buffer, m_Buffer);
            if (t_Buffer != IntPtr.Zero)
            {
                CsConvert72.RECT rct = new CsConvert72.RECT();
                rct.left = 0; rct.right = WIDTH; rct.top = 0; rct.bottom = HEIGHT;
                uint m_ImageOrg = 0;
                Hdv72.GetImageOrientation(DEVICESNUMBER, out m_ImageOrg);
                CsConvert72.Rgb24ToRgb24(WIDTH, HEIGHT, d_Buffer, t_Buffer, rct, m_ImageOrg);


                Bitmap f_Bmp = new Bitmap(WIDTH, HEIGHT, ((WIDTH * 3) + 3) & ~3,
                                            PixelFormat.Format24bppRgb, d_Buffer);
                return f_Bmp;
            }
            return null;
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
                if (Hdv72.GetImageStream(Number, out tempbuf) == 0)
                {
                    Interlocked.Exchange(ref m_Buffer, tempbuf);
                }
            }
        }
    }

    public class CsConvert72
    {
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        private CsConvert72()
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
