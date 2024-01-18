using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine((991).ToString("X").PadLeft(5, '0'));
            Console.ReadKey();
        }
    }
}
