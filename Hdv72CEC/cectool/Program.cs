using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cectool
{
    class Program
    {
        static uint DEVICE = 0;
        static void Main(string[] args)
        {
            uint Count = 0;
            uint ID = 0;
            CheckErr(Hdv72.GetDeviceCount(out Count));
            info("Device Count:" + Count.ToString());
            CheckErr(Hdv72.GetDeviceID(DEVICE, out ID));
            info("Device ID:" + ID.ToString());
            CheckErr(Hdv72.DeviceOpen(DEVICE));

            CheckErr(Hdv72.SetCECPowerUp(DEVICE, 1));
            CheckErr(Hdv72.SetCECLogicalAddress(DEVICE, 0, Convert.ToUInt32("f0", 16)));
            byte id = detect();
            if(id == 0x00)
            {
                Console.WriteLine("CEC not ready!");
                return;
            }
            List<string> items = coverage();
            foreach(string item in items)
            {
                string[] cells = item.Split(',');
                byte[] data = String2Bytes(cells[0], id);
                byte[] dest = String2Bytes(cells[1], id);
                info($"cells[2]:{cells[2]}");
                int st = int.Parse(cells[2].Split('-')[0]);
                int ed = int.Parse(cells[2].Split('-')[1]);
                Send(data);
                for(int i = 0; i < 5; i++)
                {
                    // for 40:83
                    byte[] wrong = String2Bytes("XX 83 00", id);
                    Thread.Sleep(1000);
                    byte[] recv = Read();
                    if (Compare(recv, dest, st, ed)) break;
                    if (Compare(recv, wrong, st, 2)) Send(data);
                    Thread.Sleep(10000);
                }
            }
            Console.WriteLine("finished");
        }

        static byte[] String2Bytes(string str,byte zero)
        {
            string[] source = str.Split(' ');
            byte[] data = new byte[source.Length];
            for (int i = 1; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(source[i], 16);
            }
            data[0] = zero;
            return data;
        }

        static bool Compare(byte[] source, byte[] dest, int s, int e)
        {
            try
            {
                for (int i = s; i <= e; i++)
                {
                    if (source[i] != dest[i]) return false;
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        static List<string> coverage()
        {
            string txt = File.ReadAllText("coverage.csv").Trim();
            List<string> test = new List<string>();
            foreach(string l in txt.Split('\n'))
            {
                if ((!l.Trim().StartsWith("#")) && l.Split(',').Length == 4)
                {
                    test.Add(l.Trim());
                }
            }
            return test;
        }

        static void info(string msg)
        {
            Console.WriteLine(
                string.Format("[{0}]{1}\n",
                DateTime.Now.ToString("HH:mm:ss"),
                msg.Trim())
                );
        }

        static void CheckErr(int hr)
        {
            string errcode = hr.ToString();
            string errstring = string.Empty;
            if (hr < 0)
            {
                errstring = Hdv72.GetErrorText(hr);
                info(errstring);
            }
        }

        static byte detect()
        {
            byte[] pbk_id = { 0x04, 0x08, 0x09, 0x0B };
            for(int i = 0; i < pbk_id.Length; i++)
            {
                Send(new byte[] { pbk_id[i], 0x89, 0x80 });
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(600);
                    byte[] recv = Read();
                    if(recv[1] == 0x89 && recv[2] == 0x81)
                    {
                        return pbk_id[i];
                    }
                }
            }
            return 0x00;
        }

        static void Send(byte[] data)
        {
            string str = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i].ToString("X2") + ":";
            }
            str = str.Substring(0, str.Length - 1);
            Console.WriteLine($"Send:{str}");
            int hr = Hdv72.WriteCECCommand(DEVICE, data, (uint)data.Length);
            CheckErr(hr);
        }

        static byte[] Read()
        {
            byte[] data = new byte[16];
            string str = string.Empty;
            int hr = Hdv72.ReadCECCommand(DEVICE, 0, data, (uint)data.Length);
            if (hr < 0)
            {
                //Console.WriteLine($"hr:{hr}");
                CheckErr(hr);
                return new byte[16];
            }
            bool print = false;
            //Console.WriteLine( data);
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0) print = true;
                str += data[i].ToString("X2") + ":";
            }
            str = str.Substring(0, str.Length - 1);
            //if (str.Contains("40:83:00")) 
            //{
            //    break;
            //}
            //Console.WriteLine(str);
            if (print)
            {
                Console.WriteLine($"Get:{str}");
            }
            //else 
            //{
            //    Console.WriteLine($"Get Stream:{str}");
            //}
            return data;
            //Thread.Sleep(2000);
            //byte[] command = String2Bytes("XX 89 00", 0x04);
            //Send(command);
        }
    }
}
