using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace LustyStudy
{
    class Person
    {
        public int id = 0;
    }
    class Program
    {
        static void Main()
        {
            Person person = new Person();
            Console.WriteLine("I'm here, I am Lusty");
            Console.WriteLine(person.id);
            Console.ReadLine();
            CSV_Test csv = new CSV_Test();
            csv.readfile();
        }
    }

    class CSV_Test
    {
        public string filepath = "E:\\Projects\\Z7B\\z7b_smt_fa\\Z7B\\1.csv";
        public void readfile()
        {
            CSV_Test csv = new CSV_Test();
            using (TextFieldParser parser = new TextFieldParser(csv.filepath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(','.ToString());

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    Console.WriteLine(fields[2]);
                    foreach (string field in fields)
                    {
                        Console.Write(field + " // ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}


