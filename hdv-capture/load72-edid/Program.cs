using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace load72_edid
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "C:\\Program Files\\ADLINK\\HDV72\\EDID\\ADLINK_HDV72_EDID_v20170309182006.txt";
            if (args.Length == 1)
            {
                filename = args[0];
            }
            if (!File.Exists(filename))
            {
                Console.WriteLine($"File not exist:{filename}");
                return;
            }
            int r = Capture.Open();
            if(r != 0)
            {
                Console.WriteLine($"Open device error:{r}");
                return;
            }
            Capture.LoadEdid(filename);
            Capture.Close();
        }
    }
}
