using System;
using classes.Contracts;

namespace classes
{
    public class Transaction : ITransaction
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; }
        public string Notes { get; set; }

        public Transaction(decimal amount, DateTime date, string note)
        {
            this.Amount = amount;
            this.Date = date;
            this.Notes = note;
        }
    }
}                           