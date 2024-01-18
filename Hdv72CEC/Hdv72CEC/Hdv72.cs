using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// C# class for Hdv72 library
/// </summary>
public class Hdv72
{
    public const int MAX_DEVICE_NUM = 16;

    public enum SourceList
    {
        HDMI_SOURCE = 0
    }

    public enum OutputFormat
    {
        FORMAT_RGB24_OUTPUT = 0,
        FORMAT_BGR30_OUTPUT = 1,
        FORMAT_RGB36_OUTPUT = 2,
        FORMAT_YUV8_OUTPUT = 3,
        FORMAT_YUV10_OUTPUT = 4,
        FORMAT_YUV12_OUTPUT = 5,
        FORMAT_YUY2_OUTPUT = 6,
        FORMAT_YU10_OUTPUT = 7,
        FORMAT_YU12_OUTPUT = 8
    }

    public enum AudioSourceList
    {
        HDMI_AUDIO_SOURCE = 0,
        SPDIF_TOSLINK_AUDIO_SOURCE = 1,
        SPDIF_RCA_AUDIO_SOURCE = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SENSOR_RESOLUTION
    {
        public uint Width;
        public uint Height;
        public uint FrameRate;
        public uint Interlace;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SENSOR_PROPERTIES
    {
        // name
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Name;

        // video setting
        public uint Width;
        public uint Height;
        public uint FrameRate;
        public uint Interlace;

        // video timing
        public uint Hf;		// horizontal frequency
        public uint HTotal;	// horizontal total line
        public uint Hsw;	// horizontal sync width
        public uint Hbp;	// horizontal back porch
        public uint Vf;		// vertical frequency
        public uint VTotal;	// vertical total line
        public uint Vsw;	// vertical sync width
        public uint Vbp;	// vertical back porch

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RESOLUTION_CAPABILITIES
    {
        public uint NumHdmiResolution;
        public IntPtr HdmiResolutions;
        public uint NumMhlResolution;
        public IntPtr MhlResolutions;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_FORMATS
    {
        public uint NumChannels;
        public IntPtr Channels;

        public uint NumBitsPerSample;
        public IntPtr BitsPerSamples;

        public uint NumSamplesPerSec;
        public IntPtr SamplesPerSecs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_CAPABILITIES
    {
        public AUDIO_FORMATS HdmiAudioFormats;

        public AUDIO_FORMATS Spdif1AudioFormats;

        public AUDIO_FORMATS Spdif2AudioFormats;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AUDIO_STREAM_INFO
    {
        public IntPtr Data;
        public uint Size;
    }

    //
    // EventType = 0 : Frame event
    //           = 1 : DI event
    //
    public delegate void Hdv72CALLBACK(uint Number, uint EventType);

    //
    // Device Control
    //
    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceCount")]
    public static extern int GetDeviceCount(out uint Count);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_DeviceOpen")]
    public static extern int DeviceOpen(uint Number);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_DeviceClose")]
    public static extern int DeviceClose(uint Number);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceVendorName")]
    public static extern int GetDeviceVendorName(StringBuilder Name);
    public static string GetDeviceVendorName()
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetDeviceVendorName(buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceModelName")]
    public static extern int GetDeviceModelName(uint Number, StringBuilder Name);
    public static string GetDeviceModelName(uint Number)
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetDeviceModelName(Number, buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceVersion")]
    public static extern int GetDeviceVersion(uint Number, StringBuilder Version);
    public static string GetDeviceVersion(uint Number)
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetDeviceVersion(Number, buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceFirmwareVersion")]
    public static extern int GetDeviceFirmwareVersion(uint Number, StringBuilder Version);
    public static string GetDeviceFirmwareVersion(uint Number)
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetDeviceFirmwareVersion(Number, buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDriverVersion")]
    public static extern int GetDriverVersion(uint Number, StringBuilder Version);
    public static string GetDriverVersion(uint Number)
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetDriverVersion(Number, buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetLibraryVersion")]
    public static extern int GetLibraryVersion(StringBuilder Version);
    public static string GetLibraryVersion()
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetLibraryVersion(buf) == 0)
            return buf.ToString();
        return null;
    }

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDeviceID")]
    public static extern int GetDeviceID(uint Number, out uint ID);

    //
    // Image Format Control
    //

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetChannel")]
    public static extern int SetChannel(uint Number, uint Channel);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetChannel")]
    public static extern int GetChannel(uint Number, out uint Channel);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetSensorFormat")]
    public static extern int SetSensorFormat(uint Number, uint Format);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSensorFormat")]
    public static extern int GetSensorFormat(uint Number, out uint Format);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSensorWidth")]
    public static extern int GetSensorWidth(uint Number, out uint Width);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSensorHeight")]
    public static extern int GetSensorHeight(uint Number, out uint Height);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetOutputFormat")]
    public static extern int SetOutputFormat(uint Number, uint Format);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetOutputFormat")]
    public static extern int GetOutputFormat(uint Number, out uint Format);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetHdmiSensorResolution")]
    public static extern int GetHdmiSensorResolution(uint Number, out SENSOR_RESOLUTION Resolution);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetVideoCapabilities")]
    public static extern int GetVideoCapabilities(uint Number, out RESOLUTION_CAPABILITIES Caps);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetDetectedSensorFormat")]
    public static extern int GetDetectedSensorFormat(uint Number, out int Format);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetImageOrientation")]
    public static extern int SetImageOrientation(uint Number, uint Value);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetImageOrientation")]
    public static extern int GetImageOrientation(uint Number, out uint Value);

    //
    // Event & Callback
    //

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetEventSelector")]
    public static extern int SetEventSelector(uint Number, uint Mode);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetEventSelector")]
    public static extern int GetEventSelector(uint Number, out uint Mode);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetEventHandle")]
    public static extern int SetEventHandle(uint Number, IntPtr Handle);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetEventHandle")]
    public static extern int SetEventHandle(uint Number, SafeWaitHandle Handle);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetEventHandle")]
    public static extern int GetEventHandle(uint Number, out IntPtr Handle);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetEventHandle")]
    public static extern int GetEventHandle(uint Number, out SafeWaitHandle Handle);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCallbackSelector")]
    public static extern int SetCallbackSelector(uint Number, uint Mode);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetCallbackSelector")]
    public static extern int GetCallbackSelector(uint Number, out uint Mode);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetCallback")]
    public static extern int SetCallback(uint Number, Hdv72CALLBACK Fun);

    //
    // Acquisition Control
    //

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAcquisitionFrameCount")]
    public static extern int SetAcquisitionFrameCount(uint Number, uint Count);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAcquisitionFrameCount")]
    public static extern int GetAcquisitionFrameCount(uint Number, out uint Count);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_AcquisitionStart")]
    public static extern int AcquisitionStart(uint Number);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_AcquisitionStop")]
    public static extern int AcquisitionStop(uint Number);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_OneShot")]
    public static extern int OneShot(uint Number, uint Timeout);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetImageStream")]
    public static extern int GetImageStream(uint Number, out IntPtr Buffer);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAcquisitionStatus")]
    public static extern int GetAcquisitionStatus(uint Number, out uint Status);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAcquisitionStatistics")]
    public static extern int GetAcquisitionStatistics(uint Number, out uint Count);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSensorStatus")]
    public static extern int GetSensorStatus(uint Number, out uint Locked);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SaveImage")]
    public static extern int SaveImage(uint Number, string FileName);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioStream")]
    public static extern int GetAudioStream(uint Number, out AUDIO_STREAM_INFO StreamInfo);


    //
    // EDID
    //
    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetEdid")]
    public static extern int SetEdid(uint Number, uint Port, ushort Offset, byte[] Data, ushort Length);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetEdid")]
    public static extern int GetEdid(uint Number, uint Port, ushort Offset, byte[] pData, ushort Length);


    //
    // others
    //

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetErrorText")]
    public static extern int GetErrorText(int code, StringBuilder Text);
    public static string GetErrorText(int code)
    {
        StringBuilder buf = new StringBuilder(250, 250);
        if( GetErrorText(code, buf) == 0)
            return buf.ToString();
        return null;
    }
    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetSerialNumber")]
    public static extern int GetSerialNumber(uint Number, Byte[] SerialNumber);
    //public static string GetSerialNumber(uint Number)
    //{
    //    Byte[] serialNumber = new Byte[10];
    //    if( GetSerialNumber(Number, serialNumber) == 0)
    //    {
    //        string strSerialNumber = System.Text.Encoding.UTF8.GetString(serialNumber);
    //        return strSerialNumber;
    //    }
    //    return null;
    //}

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetQCText")]
    public static extern int SetSerialNumber(uint Number, Byte[] SerialNumber, uint Length, uint AuthCode);


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
    // Audio Format Control
    //
    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAudioInput")]
    public static extern int SetAudioInput(uint Number, uint Channel);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioInput")]
    public static extern int GetAudioInput(uint Number, out uint Channel);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAudioChannels")]
    public static extern int SetAudioChannels(uint Number, uint Channels);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioChannels")]
    public static extern int GetAudioChannels(uint Number, out uint Channels);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAudioSamplesPerSec")]
    public static extern int SetAudioSamplesPerSec(uint Number, uint Value);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioSamplesPerSec")]
    public static extern int GetAudioSamplesPerSec(uint Number, out uint Value);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SetAudioBitsPerSample")]
    public static extern int SetAudioBitsPerSample(uint Number, uint Value);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioBitsPerSample")]
    public static extern int GetAudioBitsPerSample(uint Number, out uint Value);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_GetAudioCapabilities")]
    public static extern int GetAudioCapabilities(uint Number, out AUDIO_CAPABILITIES Caps);


    //
    // Audio Saving
    //
    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_InitialAudio")]
    public static extern void InitialAudio(uint Number, uint Channels, uint nBitsPerSample, uint nSamplesPerSec);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SaveAudioStart")]
    public static extern int SaveAudioStart(string FileName);

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_SaveAudioStop")]
    public static extern int SaveAudioStop();

    [DllImport("Hdvs.dll", EntryPoint = "Hdv72_CloseAudio")]
    public static extern void CloseAudio();

}
