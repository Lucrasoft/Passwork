using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{
    public interface IAttachment
    {

        public string Id { get; }

        /// <summary>
        /// The name (or filename) of the attachment
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the decrypted byte array of the attachment
        /// </summary>
        /// <returns></returns>
        Task<byte[]> GetContent();

        /// <summary>
        /// Deletes this attachment
        /// </summary>
        /// <returns>True is delete was succesfull</returns>
        Task<bool> Delete();
    }
}
