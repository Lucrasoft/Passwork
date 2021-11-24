using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{

    /// <summary>
    /// Captures the response from \vault\{id}
    /// </summary>
    internal class VaultSingleItem : VaultListItem
    {
        public string passwordCrypted { get; set; }
        public string domainMaster { get; set; }

        public VaultSingleItem()
        {
            foldersAmount = 0;
            passwordsAmount = 0;
        }
    }
}
