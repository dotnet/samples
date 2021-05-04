using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que01
{
    public struct Calculate
    {
        private int Number1;
        
        public int No1 //property
        {
            get { return this.Number1; }
            set { this.Number1 = value; }
        }



        public Calculate(int No1) //constructor
        {
            this.Number1 = No1;

        }
 

        public void Square()
        {
            int sqr = Number1 * Number1;
            Console.WriteLine("Square:{0}", sqr);
        }
        public void Cube()
        {
            int c = Number1 * Number1 * Number1;
            Console.WriteLine("Cube:{0}", c);

        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a number");
            int N = int.Parse(Console.ReadLine());
            Calculate c1 = new Calculate(N);
          
            c1.Square();
           c1.Cube();
           
            Console.ReadKey();
        }
    }
}
