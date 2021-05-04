using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2_Que05
{
    class BooksDemo
    {
        public string[] S = {"BookName","Author","Publisher","Price"};
        public string[,] BookDetails = new string[2, 4];

        public void setBookDetails(int id, string BookTitle, string author,string publisher, string price)
        {
            this.BookDetails[id, 0] = BookTitle;
            this.BookDetails[id, 1] = author;
            this.BookDetails[id, 2] = publisher;
            this.BookDetails[id, 3] = price;
        }
        public void Display()
        {
            for(int i=0;i<2;i++)
            {
                for(int j=0;j<4;j++)
                {
                    Console.WriteLine("{0}:{1}", S[j],this.BookDetails[i,j]);
                    
                }
            }
        }
        
 




    }
}
