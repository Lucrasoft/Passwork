using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{
    class PasswordListItem
    {
        public string id { get; set; } = null;
        public string name { get; set; }
        public string login { get; set; }
        public string description { get; set; }
        public int? color { get; set; }
        public string url { get; set; }
        public string[] tags { get; set; }
        public string vaultId { get; set; }
        public string folderId { get; set; } = null;
    }

    internal class PasswordCustomItem
    {
        public string name { get; set;  }
        public string value { get; set; }
        public string type { get; set; }
    }

    
}
