using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jiecheng
{
    class jiecheng
    {
        public int Jc(int num)
        {

            return num > 0 ? num * Jc(num - 1) : 1;  //如果num>0则返回num *Jc(num - 1)，否则返回1
        }
    }




    class excutejiecheng

    {
        static void Main(string[] args)
        {
            jiecheng n = new jiecheng();
            int result = n.Jc(Convert.ToInt16(Console.ReadLine()));
            Console.WriteLine("result is {0}", result);
            Console.ReadKey();
        }
    }
}