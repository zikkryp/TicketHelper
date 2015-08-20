using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketHelper.Models;

namespace TicketHelper.Utils
{
    public class TestHistory : Notifier
    {
        private List<Ticket> items;

        public List<Ticket> Items
        {
            get
            {
                return items;
            }

            set
            {
                items = value;
                NotifyPropertyChanged("Items");
            }
        }
    }
}
