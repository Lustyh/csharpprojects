using System;
namespace UserDefinedException
{
    class TestTemperature
    {
        static void Main(string[] args)
        {
            Temperature temp = new Temperature();
            try
            {
                temp.showTemp();
                DriveInfo driveInfo = new DriveInfo("D");
                Console.WriteLine(driveInfo.DriveType);
                
            }
            catch (TempIsZeroException e)
            {
                Console.WriteLine("TempIsZeroException: {0}", e.Message);
            }
            Console.ReadKey();
        }
    }
}
public class TempIsZeroException : ApplicationException
{
    public TempIsZeroException(string message)
    {
    }
}
public class Temperature
{
    
    
    public void showTemp()
    {
        Console.WriteLine("请输入温度");
        int temperature = Convert.ToInt32(Console.ReadLine());
        if (temperature == 0)
        {
            throw (new TempIsZeroException("Zero Temperature found"));

        }
        else
        {
            Console.WriteLine("Temperature: {0}", temperature);
        }
    }
}