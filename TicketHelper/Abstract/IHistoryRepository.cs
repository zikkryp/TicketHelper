using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketHelper.Models;

namespace TicketHelper.Abstract
{
    public interface IHistoryRepository
    {
        IList<Ticket> Tickets { get; }
    }
}
