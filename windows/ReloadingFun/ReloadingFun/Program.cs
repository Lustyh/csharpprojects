using System;
namespace ReloadingTest
{
    class Print
    {
        public Print(int i)
        {
            Console.WriteLine(i);
        }
        public Print(double i)
        {
            Console.WriteLine(i);
        }
        public Print(string i)
        {
            Console.WriteLine(i);
        }
    }
    class Program
    {
        public static void Main()
        {
            Print p = new Print(5);
            Print p2 = new Print(10);
        }
    }
}