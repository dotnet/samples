using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Q4
{
    class Program
    {
       public class SchoolDemo
        {

            int rollnumber;
            String StudentName;
            byte age;
            char gender;
            DateTime dateOfBirth;
            String address;
            float percentage;

            public void readData()
            {
                Console.WriteLine("Input the student details:");
                
                Console.WriteLine("Enter Rollno:");
                rollnumber = Int32.Parse(Console.ReadLine());

                Console.WriteLine("Enter Student Name:");
                StudentName = Console.ReadLine();

                Console.WriteLine("Enter age:");
                age = byte.Parse(Console.ReadLine());

                Console.WriteLine("Enter Gender M/F:");
                gender = char.Parse(Console.ReadLine());

                Console.WriteLine("Enter Date of Birth in DD/MM/YYYY:");
                dateOfBirth = Convert.ToDateTime(Console.ReadLine());

                Console.WriteLine("Enter address:");
                address = Console.ReadLine();

                Console.WriteLine("Enter the percentage:");
                percentage = float.Parse(Console.ReadLine());

            }

            public void DisplayData()
            {
                Console.WriteLine();
                Console.WriteLine("The student details you entered:");
                Console.WriteLine($"Rollno: {rollnumber}");
                Console.WriteLine($"Student Name: {StudentName}");
                Console.WriteLine($"Enter age: {age}");
                Console.WriteLine($"Enter Gender M/F: {dateOfBirth}");
                Console.WriteLine($"Enter Date of Birth in DD/MM/YYYY:{dateOfBirth}");
                Console.WriteLine($"Enter address:{address} ");
                Console.WriteLine($"Enter the percentage:{percentage}");
            }
        }
        static void Main(string[] args)
        {
            SchoolDemo student = new SchoolDemo();
            student.readData();
            student.DisplayData();
            Console.ReadKey();
        }
    }
}
