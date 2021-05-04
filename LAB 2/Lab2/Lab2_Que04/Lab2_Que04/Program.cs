using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que04
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductDemo p1 = new ProductDemo();
            p1.Getdata();
            p1.display();
            p1.Amount();
            Console.ReadKey();
        }
    }
}
