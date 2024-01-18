using System;
using first_name;
using second_name;
class Program
{
    static void Main()
    {
        first adb = new first();
        second efg = new second();
        adb.adb();
        efg.efg();
        Console.ReadKey();
    }
}

namespace first_name
{
    class first
    {
        public void adb()
        {
            Console.WriteLine("This is adb function");
        }
    }
}

namespace second_name
{
    class second
    {
        public void efg()
        {
            Console.WriteLine("This is efg function");
        }
    }
}