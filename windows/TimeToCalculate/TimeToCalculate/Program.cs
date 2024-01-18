using System;
namespace Loops 
{
    class Program
    {
        static void Main()
        {
            int i,j;

            for (i = 1; i < 10; i++)
            {
                for (j = 1; j <= i; j++)
                {   
                    Console.Write("   {0}x{1}={2}", i, j, i * j);
                    Console.Write("   ");
                }
                Console.WriteLine("");
            }
            Console.ReadLine();
        }
    }
}