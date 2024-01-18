using System;

namespace GetSetApplication
{
    class Person
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Age  { get; set; }
        public string email { get; set;  }
    }
    class program
    {
        static void Main()
        {
            Person boy = new Person();
            boy.Name = "Lusty";
            Console.WriteLine(boy.Name);
            boy.Age = 21;
            Console.WriteLine(boy.Age);
            boy.email = "lusty.huang@quantacn.com";
            Console.WriteLine(boy.email);   
        }

    }
}