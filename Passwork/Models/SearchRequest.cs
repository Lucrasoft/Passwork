using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{
    class SearchRequest
    {
        public string query { get; set; } = null;
        public string vaultId { get; set; } = null;
        public int[] colors { get; set; } = null;
        public string[] tags { get; set; } = null;
    }
}
