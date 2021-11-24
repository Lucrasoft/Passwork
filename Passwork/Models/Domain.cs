using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{
    class Domain
    {
        public string domainId { get; set; }
        public string mpCrypted { get; set; }
        public string access { get; set; }
        public bool selected { get; set; }
    }
}
