using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{

    internal class Folder : IFolder
    {
        private readonly IConnection conn;
        private FolderListItem folderListItem;

        public string Id => folderListItem.id;
        public string Name => folderListItem.name;

        public int AmountFolders => folderListItem.foldersAmount;
        public int AmountPasswords => folderListItem.passwordsAmount;


        public Folder(IConnection conn, FolderListItem folderListItem)
        {
            this.conn = conn;
            this.folderListItem = folderListItem;
        }

        public async Task Refresh()
        {
            folderListItem = await conn.Get<FolderListItem>($"folders/{Id}");
        }

        public async Task<IFolder> AddFolder(string name)
        {
            var folder = new FolderAdd()
            {
                vaultId = folderListItem.vaultId,
                name = name,
                parentId = folderListItem.id
            };
            var result = await conn.Post<FolderAdd, FolderListItem>("folders", folder);
            if (result.status == "success")
            {
                return await GetFolderById(result.data.id);
            }
            return null;          
        }

        public async Task<IFolder[]> GetFolders()
        {
            var result = new List<IFolder>();
            var items = await conn.Get<FolderListItem[]>($"folders/{Id}/children");
            if (items != null)
            {
                foreach (var item in items)
                {
                    result.Add(new Folder(conn, item));
                }
            }
            if (result.Count > 0) { return result.ToArray(); }
            return null;
        }

        public async Task<IFolder> GetFolderByName(string name)
        {
            var items = await conn.Get<FolderListItem[]>($"folders/{Id}/children");
            if (items == null) return null;
            var item = items.Where(x => x.name == name).FirstOrDefault();
            if (item == null) return null;
            return new Folder(conn, item);
        }

        public async Task<bool> Delete()
        {
            var result = await conn.Delete<string>($"folders/{Id}");
            return (result.status == "success");
        }

        public IPasswordNew CreatePassword()
        {
            return new Password(conn, new PasswordListItem { folderId = Id, vaultId = folderListItem.vaultId });
        }

        public async Task<IFolder> Rename(string name)
        {
            var folder = new FolderAdd()
            {
                vaultId = folderListItem.vaultId,
                name = name,
                parentId = folderListItem.id
            };
            var result = await conn.Put<FolderAdd, FolderListItem>($"folders/{Id}", folder);
            if (result.status == "success")
            {
                this.folderListItem = result.data;
                return this;
            }
            return null;
        }

        public async Task<IPassword[]> GetPasswords()
        {
            var list = await conn.Get<PasswordListItem[]>($"folders/{folderListItem.id}/passwords");
            var result = new List<Password>();
            foreach (var listitem in list)
            {
                result.Add(new Password(conn, listitem));
            }
            return result.ToArray();
        }

       
        private async Task<IFolder> GetFolderById(string id)
        {
            var result = await conn.Get<FolderListItem>($"folders/{id}");
            if (result != null) return new Folder(conn, result);
            return null;
        }

        private async Task<IPassword> GetPasswordById(string id)
        {
            var result = await conn.Get<PasswordListItem>($"passwords/{id}");
            if (result != null) return new Password(conn, result);
            return null;
        }

    }
}
