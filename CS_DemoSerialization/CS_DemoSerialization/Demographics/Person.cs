using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_DemoSerialization.Demographics
{
    [Serializable]
    public  class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        //public string MiddleName { get; set; }

        [NonSerialized]
        private string  middleName;

        public string  MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }


        public string LastName { get; set; }

        public DateTime  DateOfBirth { get; set; }

        public int Age
        {
            get
            {
                var age=new DateTime (DateTime.Now.Subtract(DateOfBirth).Ticks).Year - 1;
                return age;
            }
        }

    }
}
