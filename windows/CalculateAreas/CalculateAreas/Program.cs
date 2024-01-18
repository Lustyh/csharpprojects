using System;
namespace RectangleApplication
{
    class calculateAreas
    {
        int lenth;
        int width;

        public void SetValue()
        {
            lenth = 10;
            width = 5;
        }

        public int GetAreas()
        {
            return lenth * width;
        }
        public void Display()
        {
            Console.WriteLine("Lenth: {0}",lenth);
            Console.WriteLine("Width: {0}",width);
            Console.WriteLine("Areas: {0}",GetAreas());
        }
    }

    class ExecuteFunction
    {
        static void Main()
        {
            calculateAreas r = new calculateAreas();
            r.SetValue();
            r.Display();
            //Console.ReadLine();
        }
    }
}