using System;
using System.Collections.Generic;

namespace classes.Contracts
{
    public interface IBank
    {
        public string Number { get; }

        public string Owner { get; }

        public decimal Balance { get; }
    }
}