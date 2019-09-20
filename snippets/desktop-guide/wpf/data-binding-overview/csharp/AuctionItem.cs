using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace bindings
{
    public class AuctionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int CurrentPrice = 0;
    }
}
