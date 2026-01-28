using System;
namespace classes.Contracts
{
    public interface ITransaction
    {
        public decimal Amount { get; }

        public DateTime Date { get; }

        public string Notes { get; }
    }
}