using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Demo_Serialization.DemoGraphics
{
    [Serializable]
    public class Person
    {
        public int Id { get; set; }//prop double tab

        public string FirstName { get; set; }
        public string MiddleName { get; set; } 
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age 
        {
            get
            {
                var age = new DateTime(DateTime.Now.Subtract(DateOfBirth).Ticks).Year - 1;
                return age;
            }
                
        }


    }
}
