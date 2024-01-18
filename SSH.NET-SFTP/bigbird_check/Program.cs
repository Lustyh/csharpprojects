using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bigbird_check
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var client = new SshClient("10.97.1.54", "bigbird", "qmb@123"))
            {
                client.Connect();
                string ret1 = client.RunCommand("ping google.com -c 3").Result;
                Console.WriteLine(ret1);
                string ret2 = client.RunCommand("find /home/bigbird/fileservice/scout/data/ -type f | wc -l").Result;
                Console.WriteLine("log files count:"+ret2);
            }
        }
    }
}
