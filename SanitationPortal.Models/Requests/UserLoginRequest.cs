using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanitationPortal.Models.Requests
{
    public class UserLoginRequest
    {
        public int EmployeeId { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}
