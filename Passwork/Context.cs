using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{

    class Context : IContext
    {

        private IConnection conn;



        public Context(IConnection conn)
        {
            this.conn = conn;
        }

        public string LastError
        {
            get
            {
                if (this.conn != null) return this.conn.LastError;
                return "";
            }
        }

        public IPasswordQuery Query()
        {
            return new PasswordQuery(conn);
        }

        public async Task<int[]> GetColors()
        {
           return await conn.Get<int[]>("/vaults/colors");
        }

        public async Task<string[]> GetTags()
        {
            return await conn.Get<string[]>("vaults/tags");
        }

        public async Task<IVault[]> GetVaults()
        {
            var vaultitems = await conn.Get<VaultListItem[]>("vaults/list");
            if (vaultitems==null) { return null; }

            var result = new List<Vault>();
            foreach (var item in vaultitems)
            {
                result.Add(new Vault(conn, item));
            }
            return result.ToArray();
        }

        public async Task<IVault> AddVault(string name, bool isPrivate)
        {
            string vaultId;
            if (isPrivate)
            {
                if (conn.WithMasterPassword)
                {
                    vaultId = await AddPrivateVaultMasterpass(name, conn.MasterPassword);
                } else
                {
                    vaultId = await AddPrivateVault(name);
                }

            } else
            {
                if (conn.WithMasterPassword)
                {
                    vaultId = await AddOrganizationVaultMasterPass(name, conn.MasterPassword);
                }
                else
                {
                    vaultId = await AddOrganizationVault(name);
                }

            }
            if (string.IsNullOrEmpty(vaultId)) return null;
            return await GetVaultByID(vaultId);

        }

        private async Task<string> AddPrivateVault(string name)
        {

            var vaultAdd = new VaultAddPers()
            {
                name = name,
                mpCrypted = "YWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWE",
                passwordCrypted = "YWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWE",
                passwordHash = "ffe054fe7ae0cb6dc65c3af9b61d5209f439851db43d0ba5997337df154668eb",
                salt = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
            };
            var response = await conn.Post<VaultAddPers, string>("vaults", vaultAdd);
            if (response.status == "success")
            {
                return response.data;
            }
            return "";
        }

        private async Task<string> AddOrganizationVault(string name)
        {
            var domain = await GetDomain();
            if (domain == null) return null;
            var vaultAdd = new VaultAddOrg()
            {
                name = name,
                domainId = domain.domainId,
                mpCrypted = "YWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWE",
                passwordHash = "ffe054fe7ae0cb6dc65c3af9b61d5209f439851db43d0ba5997337df154668eb",
                salt = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
            };
            var response = await conn.Post<VaultAddOrg, string>("vaults", vaultAdd);
            if (response.status == "success")
            {
                return response.data;
            }
            return "";
        }

        private async Task<string> AddOrganizationVaultMasterPass(string name, string masterPassword)
        {
            var domain = await GetDomain();
            if (domain == null) return null;

            var groupPwd = CryptoUtils.GenerateString(32);
            var salt = CryptoUtils.GenerateString(32);

            var domainMaster = CryptoUtils.Decode(domain.mpCrypted, masterPassword);

            var vaultAdd = new VaultAddOrg()
            {
                name = name,
                domainId = domain.domainId,
                mpCrypted = CryptoUtils.Encode(groupPwd, domainMaster),
                passwordHash = CryptoUtils.Hash(groupPwd + salt),
                salt = salt
            };
            var response = await conn.Post<VaultAddOrg, string>("vaults", vaultAdd);
            if (response.status == "success")
            {
                return response.data;
            }
            return "";
        }

        private async Task<string> AddPrivateVaultMasterpass(string name, string masterPassword)
        {
            var domain = await GetDomain();
            if (domain == null) return null;

            var groupPwd = CryptoUtils.GenerateString(32);
            var salt = CryptoUtils.GenerateString(32);

            var domainMaster = CryptoUtils.Decode(domain.mpCrypted, masterPassword);

            var vaultAdd = new VaultAddPers()
            {
                name = name,
                mpCrypted = CryptoUtils.Encode(groupPwd, domainMaster),
                passwordCrypted = CryptoUtils.Encode(groupPwd, masterPassword),
                passwordHash = CryptoUtils.Hash(groupPwd + salt),
                salt = salt
            };
            var response = await conn.Post<VaultAddPers, string>("vaults", vaultAdd);
            if (response.status == "success")
            {
                return response.data;
            }
            return "";
        }

        public async Task<IVault> GetVaultByName(string name)
        {
            name = name.ToLowerInvariant();
            var items = await conn.Get<VaultListItem[]>("vaults/list");
            var item = items?.Where(x => x.name.ToLowerInvariant() == name).FirstOrDefault();
            if (item != null)
            {
                return new Vault(conn, item);
            }
            return null;
        }


        public async Task<IFolder> GetFolderByID(string id)
        {
            var item = await conn.Get<FolderListItem>($"folders/{id}");
            if (item!=null)
            {
                return new Folder(conn, item);
            }
            return null;
        }


        public async Task<IVault> GetVaultByID(string id)
        {
            var items = await conn.Get<VaultListItem[]>("vaults/list");
            var item = items?.Where(x => x.id == id).FirstOrDefault();
            if (item != null)
            {
                return new Vault(conn, item);
            }
            return null;
        }

        public async Task<IPassword> GetPasswordByID(string id)
        {
            var singleitem = await conn.Get<PasswordSingleItem>($"passwords/{id}");
            return new Password(conn, singleitem);
        }

        public async Task<IPassword[]> GetPasswordsRecent()
        {
            var items = await conn.Get<PasswordListItem[]>($"passwords/recent");
            if (items == null) return null;

            var result = new List<IPassword>();
            foreach (var item in items)
            {
                result.Add(new Password(conn, item));
            }
            return result.ToArray();
        }

        public async Task<IPassword[]> GetPasswordsFavorite()
        {
            var items = await conn.Get<PasswordListItem[]>($"passwords/favorite");
            if (items == null) return null;

            var result = new List<IPassword>();
            foreach (var item in items)
            {
                result.Add(new Password(conn, item));
            }
            return result.ToArray();
        }

        private async Task<Domain> GetDomain()
        {
            return await conn.Get<Domain>("vaults/domain");
        }

    }
}
