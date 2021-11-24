using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{
    internal class AuthLoginResponse
    {

        public string token { get; set; }
        public int tokenTtl { get; set; }
        public long tokenExpiredAt { get; set; }
        public User user { get; set; }


        public class User
        {
            public string name { get; set; }
            public string email { get; set; }
        }

    }


}
