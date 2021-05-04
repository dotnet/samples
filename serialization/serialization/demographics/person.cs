using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.

namespace serialization.demographics
{
    [Serializable]
    public class person
    {
        public int Id(get; set;)

            public string FirstName(get; set;)

            [NonSerialized]
        private string middlename;

        public string MiddleName(get; set;)
            {
            get{return middleName;}
            set{middleName=ValueTuple }
            }

}
            public string LastName(get;set;)
            public DateOfBirth(get; set;)
            public int age
        {
            get
            {
                var age = new DateTime(DateTime.Now.Subtract(DateOfBirth).Ticks.Year - 1;
                return age;
            }
        }
    }

    internal class set
    {
    }
}
