using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCenter
{
    internal static class SlotMap
    {
        private static MemoryMappedFile block = null;

        public static void Initialize()
        {
            block = MemoryMappedFile.CreateNew(
                "devicedata",
                4096,
                MemoryMappedFileAccess.ReadWrite
                );
        }

        public static MemoryMappedViewAccessor Op()
        {
            return block.CreateViewAccessor();
        }

        public static SlotInfo Slot(int i)
        {
            return new SlotInfo(i);
        }

        public static string ReadString(int position,int length)
        {
            byte[] data = new byte[length];
            using (MemoryMappedViewAccessor op = Op())
            {
                op.ReadArray<byte>(position, data, 0, data.Length);
            }
            return Encoding.ASCII.GetString(data);
        }

        public static string ReadString(MemoryMappedViewAccessor op,int position,int length)
        {
            byte[] data = new byte[length];
            op.ReadArray<byte>(position, data, 0, data.Length);
            return Encoding.ASCII.GetString(data);
        }

        public static void SaveString(int position,string value)
        {
            byte[] data = Encoding.ASCII.GetBytes(value);
            using (MemoryMappedViewAccessor op = Op())
            {
                op.WriteArray<byte>(position, data, 0, data.Length);
            }
        }
        
    }

    /// <summary>
    /// | IP                    | SN                | Fixture_ID              | Slot | Status | Fail_Count |
    /// | 192.168.111.111:12345 | WIP2727105FE00001 | NHL_P3-K19_FATP-HDMI_01 | 0    | 1      | 1          |
    /// </summary>
    internal class SlotInfo
    {
        private int UNIT_BASE = 0;
        
        private const int LEN_IP = 25;
        private const int LEN_SN = 25;
        private const int LEN_FX = 50;
        private const int LEN_HD = 4;
        private const int LEN_SX = 6;
        private const int LEN_FC = 10;

        private const int OFFSET_IP = 0;
        private const int OFFSET_SN = (OFFSET_IP+LEN_IP);
        private const int OFFSET_FX = (OFFSET_SN+LEN_FX);
        private const int OFFSET_HD = (OFFSET_FX+LEN_HD);
        private const int OFFSET_SX = (OFFSET_HD+LEN_SX);
        private const int OFFSET_FC = (OFFSET_SX+LEN_FC);

        private const int UNIT_SPACE = (
            LEN_IP + LEN_SN + LEN_FX + LEN_HD + LEN_SX + LEN_FC
            );

        public SlotInfo(int i) { UNIT_BASE = UNIT_SPACE * i; }

        public string Address
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_IP, LEN_IP); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_IP, value); }
        }

        public string SN
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_SN, LEN_SN); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_SN, value); }
        }

        public string Fixture_ID
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_FX, LEN_FX); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_FX, value); }
        }

        public string Slot
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_HD, LEN_HD); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_HD, value); }
        }

        public string Status
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_SX, LEN_SX); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_SX, value); }
        }

        public string FailCount
        {
            get { return SlotMap.ReadString(UNIT_BASE + OFFSET_FC, LEN_FC); }
            set { SlotMap.SaveString(UNIT_BASE + OFFSET_FC, value); }
        }

        public string ToGrid()
        {
            string grid = string.Empty;
            using(MemoryMappedViewAccessor op = SlotMap.Op())
            {
                string ip = SlotMap.ReadString(op, UNIT_BASE + OFFSET_IP, LEN_IP);
                string sn = SlotMap.ReadString(op, UNIT_BASE + OFFSET_SN, LEN_SN);
                string fx = SlotMap.ReadString(op, UNIT_BASE + OFFSET_FX, LEN_FX);
                string hd = SlotMap.ReadString(op, UNIT_BASE + OFFSET_HD, LEN_HD);
                string sx = SlotMap.ReadString(op, UNIT_BASE + OFFSET_SX, LEN_SX);
                string fc = SlotMap.ReadString(op, UNIT_BASE + OFFSET_FC, LEN_FC);
                grid = $"|{ip}|{sn}|{fx}|{hd}|{sx}|{fc}|";
            }
            return grid;
        }
    }

}
