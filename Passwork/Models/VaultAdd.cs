using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{

    public class VaultAddPers
    {
        public string name { get; set; }
        public string mpCrypted { get; set; } 
        public string passwordCrypted { get; set; }
        public string passwordHash { get; set; }
        public string salt { get; set; }
    }

    public class VaultAddOrg 
    {
        public string name { get; set; }
        public string mpCrypted { get; set; }
        public string passwordHash { get; set; }
        public string salt { get; set; }
        public string domainId { get; set; }
    }

}
