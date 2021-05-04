using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que02
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] id = new int[5, 6];
            for(int i =0; i<5; i++)
            {
                for(int j=0; j<6;j++)
                {
                    Console.WriteLine("Enter Matrix values"); //approx 30 numbers u have to input
                    id[i, j] = Convert.ToInt32(Console.ReadLine());
                    

                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j <6; j++)
                {
                    Console.Write(id[i, j] + " ");
                    //Console.Write(Name[i,j]+ " ");
                    

                }
                Console.WriteLine("");
            }
            Console.ReadKey();

        }
    }
}
