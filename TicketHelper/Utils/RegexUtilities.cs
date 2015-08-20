using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicketHelper.Utils
{
    public class RegexUtilities
    {
        //private bool ValidateEmailID()
        //{
        //    emailfield.ChangeValidationState(ValidationState.Validating, "validating");
        //    bool isValid = isValidEmail(emailfield.Text);
        //    if (isValid)
        //    {
        //        emailfield.ChangeValidationState(ValidationState.Valid, "great!");
        //    }
        //    else
        //    {
        //        emailfield.ChangeValidationState(ValidationState.Invalid, "Invalid email address!");
        //    }
        //    return isValid;
        //}

        public static bool isValidEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }
    }
}
