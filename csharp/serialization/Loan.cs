using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace serialization
{
    [Serializable()]
    // <Snippet1>
   public class Loan : INotifyPropertyChanged
    {
        public double LoanAmount { get; set; }
        public double InterestRate { get; set; }
        public int Term { get; set; }

        private string customer;
        public string Customer
        {
            get { return customer; }
            set
            {
                customer = value;
                PropertyChanged(this,
                  new PropertyChangedEventArgs(nameof(Customer)));
            }
        }

        [field: NonSerialized()]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public Loan(double loanAmount,
                    double interestRate,
                    int term,
                    string customer)
        {
            this.LoanAmount = loanAmount;
            this.InterestRate = interestRate;
            this.Term = term;
            this.customer = customer;
        }

    }
    // </Snippet1>
}
