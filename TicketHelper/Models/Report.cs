using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHelper.Models
{
    public class Report
    {
        public Report(string type, string content)
        {
            this.Type = type;
            this.Content = content;
        }

        public string Type { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Content;
        }
    }
}
