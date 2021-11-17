using System;
using System.Collections.Generic;
using System.Text;

namespace Stack.DTOs.Requests.Users
{
    public class EditSuperAdminPasswordModel
    {
        public string Email { get; set; }
        
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

    }

}

