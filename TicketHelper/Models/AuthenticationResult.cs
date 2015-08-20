using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHelper.Models
{
    public class AuthenticationResult
    {
        public AuthenticationResult(User user, AuthenticationStatus responseStatus)
        {
            this.User = user;
            this.ResponseStatus = responseStatus;
        }

        public User User { get; private set; }
        public AuthenticationStatus ResponseStatus;
    }
}
