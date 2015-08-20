using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHelper.Models
{
    public class History : Notifier
    {
        private List<Ticket> items = new List<Ticket>();

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
