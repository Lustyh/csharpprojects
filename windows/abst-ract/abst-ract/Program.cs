using System;
namespace TryAbstruck 
{
    abstract class CalculateValue
    {
        abstract public int area();
    }
    class Retangle : CalculateValue
    {
        private int h;
        private int w;
        public void getValue(int a, int b)
        {
            h = a;
            w = b;
        }
        public override int area()
        {
            return  h *  w;
        }
    class FinalTest
    {
            static void Main()
            {
                Retangle re = new Retangle();
                re.getValue(5, 10);
                int output = re.area();
                Console.WriteLine(output);
            }
    }
    }
}