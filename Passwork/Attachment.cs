using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{
    class Attachment : IAttachment
    {

        private readonly IPassword parent;
        private readonly IConnection conn;
        private readonly AttachmentListItem listitem;

        private bool isLocked =true;
        private byte[] data;

        public string Id => listitem.id;

        public string Name => listitem.name;

        public Attachment(IConnection conn, IPassword password, AttachmentListItem item)
        {
            this.conn = conn;
            this.parent = password;
            this.listitem = item;
        }

        public async Task<byte[]> GetContent()
        {
            //decrpy the whole thing.
            await Unlock();
            return data;
        }

        public async Task<bool> Delete()
        {
            var result = await conn.Delete<string>($"passwords/{parent.Id}/attachment/{Id}");
            return (result.status == "success");
        }

        /// <summary>
        /// Unlock is similar to password unlock as in: it get's the 
        /// </summary>
        /// <returns></returns>
        private async Task Unlock()
        {
            //do not reload settings if it is already locked..
            if (!isLocked) { return; }

            //get the details about this password
            var singleitem = await conn.Get<AttachmentSingleItem>($"passwords/{parent.Id}/attachment/{Id}");

            if (conn.WithMasterPassword)
            {
                var vsi = await this.conn.Get<VaultSingleItem>($"vaults/{parent.VaultId}");
                IVault vault = new Vault(conn, vsi);

                var masterPass = await vault.GetMaster();
                var key = CryptoUtils.Decode(singleitem.encryptedKey, masterPass);

                //TODO i skipped the has check..
                //if (cryptoInterface.hash(byteCharacters) !== attachment.hash)
                //{
                //    throw "Can't decrypt attachment: hashes are not equal";
                //}
                data = CryptoUtils.DecodeFile(singleitem.encryptedData, key);
            }
            else
            {
                data = Convert.FromBase64String(Base64.Decode(singleitem.encryptedData));
            }
        }
        
    
         

        
    }
}
