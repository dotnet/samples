using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que05
{
    class Program
    {
        static void Main(string[] args)
        {
            BooksDemo b1 = new BooksDemo();
            for(int i=0;i<2;i++)
            {
                Console.WriteLine("Enter Book title");
                string _book = Console.ReadLine();

                Console.WriteLine("Enter Author name");
                string _author = Console.ReadLine();

                Console.WriteLine("Enter Publisher name");
                string _publisher = Console.ReadLine();

                Console.WriteLine("Enter Book price");
                string _price = Console.ReadLine();

                b1.setBookDetails(i, _book, _author, _publisher, _price);


            }
            b1.Display();
            Console.ReadKey();
        }
    }
}
