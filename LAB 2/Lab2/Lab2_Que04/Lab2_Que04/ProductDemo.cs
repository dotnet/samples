using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que04
{
    public class ProductDemo
    {
        public int PID;
        public string Product_name;
        public float price;
        public int quantity;
        public double amount_payable;
        
        public void Getdata()
        {
            Console.WriteLine("Enter PId");
            PID = Convert.ToInt32(Console.ReadLine());
            object o1 = PID; //boxing
            PID = (int)o1;  //unboxing

            Console.WriteLine("Enter Product Name");
            Product_name = Console.ReadLine();
            object o2 = Product_name; //boxing
            Product_name = (string)o2;  //unboxing

            Console.WriteLine("Enter Quantity of product");
            quantity = Convert.ToInt32(Console.ReadLine());
            
            Console.WriteLine("Enter price of one item");
            price = Convert.ToInt32(Console.ReadLine());
           
        }

        public void display()
        {
            Console.WriteLine("PID: {0}", PID);
            Console.WriteLine("Product Name: {0}", Product_name);
            Console.WriteLine("Quantity: {0}", quantity);
            Console.WriteLine("Price: {0}", price);
        }
        public void Amount()
        {
            amount_payable = price * quantity;
            Console.WriteLine("Amount Payable:{0}", amount_payable);

        }


    }
}
