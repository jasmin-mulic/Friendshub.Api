using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Friendshub.Application.Validators
{
    public static class UsernameValidator
    {
        public static bool IsValidUsername(string username, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username can not me empty.";
                return false;
            }

            if (username.Length > 30)
            {
                return false;
            }
            var regex = new Regex(@"^(?!.*\.\.)(?!\.)(?!.*\.$)[a-zA-Z0-9._]{1,30}$");

            if (!regex.IsMatch(username))
            {
                errorMessage = "Username can contain letters, numbers, _ and .; cannot start/end with . or have consecutive dots";
                return false;
            }

            return true;
        }
    }
}
