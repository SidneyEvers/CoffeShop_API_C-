using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema_Gerenciamento_Cafeteria.Models
{
    public class ChangePassword
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}