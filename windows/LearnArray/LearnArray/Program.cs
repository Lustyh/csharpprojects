using System;
using System.Threading;
namespace ArrayTest
{
    public class ArrayTest
    {
        static void Main()
        {
            int[,] a = new int[3, 4] {
             {0, 1, 2, 3} ,   /*  初始化索引号为 0 的行 */
             {4, 5, 6, 7} ,   /*  初始化索引号为 1 的行 */
             {8, 9, 10, 11}   /*  初始化索引号为 2 的行 */
            };
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    Console.Write(a[j, i].ToString() + '\t');
                }
                Console.WriteLine();    
            }
            
            do
            {
                DateTime date = DateTime.Now;
                Console.WriteLine(date);
                Console.WriteLine("{0:yyyy-MM-dd HH:mm:ss.fff}", date);
                Thread.Sleep(1000);
            }while (true); 
            
        }
    }
}