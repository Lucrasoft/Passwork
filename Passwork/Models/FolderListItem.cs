using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{

    public class FolderListItem
    {
        public string vaultId { get; set; }
        public string name { get; set; }
        public string parentId { get; set; }
        public string id { get; set; }
        public int foldersAmount { get; set; }
        public int passwordsAmount { get; set; }
    }
}
