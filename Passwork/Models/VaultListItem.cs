using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{

    internal class VaultListItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string access { get; set; }
        public string scope { get; set; }
        public bool visible { get; set; }
        public int foldersAmount { get; set; }
        public int passwordsAmount { get; set; }
    }
}
