using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHelper.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : {1}", this.Username, this.Id);
        }
    }
}
