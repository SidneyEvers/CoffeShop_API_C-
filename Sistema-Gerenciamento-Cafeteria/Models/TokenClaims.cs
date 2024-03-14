using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sistema_Gerenciamento_Cafeteria.Models
{
    public class TokenClaims
    {
        public string email { get; set; }
        public string role { get; set; }
    }
}