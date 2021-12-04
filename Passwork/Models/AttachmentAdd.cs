using System;
using System.Collections.Generic;
using System.Text;

namespace Passwork.Models
{
    class AttachmentAdd
    {
        public string encryptedKey { get; set; }
        public string encryptedData { get; set; }
        public string hash { get; set; }
        public string name { get; set; }
    }
}
