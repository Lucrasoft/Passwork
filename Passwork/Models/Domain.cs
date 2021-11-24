using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{

     //"domainId": "61852f92eb3bf46a3a3831e6",
     //   "access": "admin",
     //   "mpCrypted": "YWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWE=",
     //   "selected": true
    class Domain
    {
        public string domainId { get; set; }
        public string mpCrypted { get; set; }
        public string access { get; set; }
        public bool selected { get; set; }
    }
}
