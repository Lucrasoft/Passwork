using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{

    class Vault : IVault
    {
        private readonly IConnection conn;
        private VaultListItem item;
        private VaultSingleItem singleItem = null;

        public string Name => item.name;
        public string Id => item.id;
        public string Scope => item.scope;

     
        public Vault(IConnection conn, VaultListItem item)
        {
            this.conn = conn;
            this.item = item;
        }

        public async Task<int[]> GetColors()
        {
            return await conn.Get<int[]>($"/vaults/{Id}/colors");
        }

        public async Task<string[]> GetTags()
        {
            return await conn.Get<string[]>($"vaults/{Id}/tags");
        }

        //public async Task<IVault> Rename(string newname)
        //{
        //    //according to the documentation, this should work. 
        //    //we even get success, but the name property remains 
        //    var vaultRename = new VaultRename { name = newname };
        //    var result = await conn.Put<VaultRename, string>($"vaults/{Id}", vaultRename);
        //    if (result.status=="succes") {
        //        this.item.name = newname; 
        //    }
        //    return this;
        //}


        public IPasswordQuery Query()
        {
            return new PasswordQuery(conn, this.Id);
        }

        async Task<string> IVault.GetMaster()
        {
            if (!conn.WithMasterPassword) { return ""; }
            
            //the VaultListItem does not return the required fields (passwordCrypted, domainMaster) 
            //this is only included in the VaultSingleItem.
            //So we need to request the single item version of a vault.
            if (singleItem == null)
            {
                singleItem = await this.conn.Get<VaultSingleItem>($"vaults/{this.Id}");
                if (singleItem == null)
                {
                    throw new Exception("Could not request details about this vault.");
                }
            }
            
            if (Scope == "domain")
            {
                var domainMaster = CryptoUtils.Decode(singleItem.domainMaster, conn.MasterPassword);
                return CryptoUtils.Decode(singleItem.passwordCrypted, domainMaster);
            }
            else
            {
                return CryptoUtils.Decode(singleItem.passwordCrypted, conn.MasterPassword);
            }
        }


        public async Task<bool> Delete()
        {
            var result = await conn.Delete<string>($"vaults/{item.id}");
            if (result.status == "succes")
            {
                //this.conn = null;
                this.item = null;
                return true;
            }
            return false;
        }

        public IPasswordNew CreatePassword()
        {
            return new Password(conn, new PasswordListItem { folderId = null, vaultId = item.id });
        }

        public async Task<IPassword[]> GetPasswords()
        {
            var list = await conn.Get<PasswordListItem[]>($"vaults/{item.id}/passwords");
            var result = new List<Password>();
            foreach (var listitem in list)
            {
                result.Add(new Password(conn, listitem));
            }

            return result.ToArray();
        }

        public async Task<IFolder[]> GetFolders()
        {
            var result = new List<IFolder>();
            var items = await conn.Get<FolderListItem[]>($"vaults/{Id}/folders");
            if (items != null)
            {
                foreach (var itm in items)
                {
                    result.Add(new Folder(conn, itm));
                }
            }
            if (result.Count > 0) { return result.ToArray(); }
            return null;
        }

        public async Task<IFolder> GetFolderByName(string name)
        {
            var items = await conn.Get<FolderListItem[]>($"vaults/{Id}/folders");
            if (items == null) return null;
            var item = items.Where(x => x.name == name).FirstOrDefault();
            if (item == null) return null;
            return new Folder(conn, item);
        }

        public async Task<IFolder> GetFolderById(string id)
        {
            var result = await conn.Get<FolderListItem>($"folders/{id}");
            if (result != null) return new Folder(conn, result);
            return null;
        }

        public async Task<IFolder> AddFolder(string name)
        {
            var folder = new FolderAdd()
            {
                vaultId = Id,
                name = name
            };
            var result = await conn.Post<FolderAdd, FolderListItem>("folders", folder);
            if (result.status == "success")
            {

                return await GetFolderById(result.data.id);
            }
            return null;
        }

        
    }
}
