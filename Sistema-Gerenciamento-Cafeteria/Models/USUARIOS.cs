namespace Sistema_Gerenciamento_Cafeteria.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class USUARIOS
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string numeroContato { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string status { get; set; }
        public string role { get; set; }
    }
}
