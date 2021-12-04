using System;
using System.Collections.Generic;
using System.Text;

namespace Passwork.Models
{
    class AttachmentSingleItem : AttachmentListItem
    {

        public string hash { get; set; }
        public string encryptedData { get; set; }

    }
}
