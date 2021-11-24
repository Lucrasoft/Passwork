using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{

    class Password : IPassword
    {

        private IConnection conn;
        private PasswordListItem listitem;

        private bool isLocked = true;

        //private PasswordSingleItem singleitem;
        private IVault vault;

        public string Id => listitem.id;
        public string Name { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// You must first call Unlock before the Password is filled.
        /// </summary>
        public string Pass { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public int? Color { get; set; }

        /// <summary>
        /// Only available when you first call Unlock.
        /// </summary>
        public List<CustomField> CustomRecords { get; set; }


        public Password(IConnection conn, PasswordListItem item)
        {
            this.conn = conn;
            this.listitem = item;
            MapFromItem();
            if (item.id == null)
            {
                //new record so enable the custom properties
                CustomRecords = new List<CustomField>();
            }
        }

        public async Task Save()
        {
            var item = await MapToItem();
            if (item.id == null)
            {
                //it is a new record.
                await Create(item);
            }
            else
            {
                await Update(item);
            }
            isLocked = false;
        }


        public async Task Unlock()
        {
            //do not reload settings if it is already locked..
            if (!isLocked) { return; }

            //do not load , if this is a new item.
            if (listitem.id == null) { return; }

            //get the details about this password
            var singleitem = await conn.Get<PasswordSingleItem>($"passwords/{listitem.id}");

            //we also (might) need the vault for decryption
            //make sure the vault is loaded.
            if (this.vault == null) { await LoadVault(); }

            Pass = await CryptoUtils.DecryptString(singleitem.cryptedPassword, vault, conn.WithMasterPassword);

            if (CustomRecords == null) { CustomRecords = new List<CustomField>(); }
            CustomRecords.Clear();

            if (singleitem.custom != null)
            {
                foreach (var custom in singleitem.custom)
                {
                    CustomRecords.Add(new Passwork.CustomField()
                    {
                        Name = await CryptoUtils.DecryptString(custom.name, vault, conn.WithMasterPassword),
                        Value = await CryptoUtils.DecryptString(custom.value, vault, conn.WithMasterPassword),
                        Type = (CustomFieldType)Enum.Parse(typeof(CustomFieldType), await CryptoUtils.DecryptString(custom.type, vault, conn.WithMasterPassword))
                    }); ;
                }
            }
            isLocked = false;
        }

        public async Task SetFavorite(bool IsFavorite)
        {
            //cannot set favorite on unsaved record..
            if (listitem.id==null) { return; }
            //POST /passwords/{id}/favorite
            Response<object> result;
            if (IsFavorite)
            {
                result = await conn.Post<string, object>($"passwords/{Id}/favorite", null);
            } else
            {
                result = await conn.Post<string, object>($"passwords/{Id}/unfavorite", null);
            }
            return;
        }

        private async Task<PasswordSingleItem> MapToItem()
        {
            if (isLocked) { await Unlock(); }

            //make sure the vault is loaded.
            if (this.vault == null) { await LoadVault(); }

            //converts the properties back to be able to add or save it.
            var newitem = new PasswordSingleItem()
            {
                id = listitem.id,
                name = Name,
                url = Url,
                cryptedPassword = await CryptoUtils.EncryptString(Pass, this.vault, conn.WithMasterPassword),
                login = Login,
                tags = Tags,
                description = Description,
                color = Color,
                vaultId = listitem.vaultId,
                folderId = listitem.folderId
            };

            var tmp = new List<PasswordCustomItem>();
            foreach (var custom in CustomRecords)
            {
                tmp.Add(new PasswordCustomItem()
                {
                    name = await CryptoUtils.EncryptString(custom.Name, this.vault, conn.WithMasterPassword),
                    value = await CryptoUtils.EncryptString(custom.Value, this.vault, conn.WithMasterPassword),
                    type = await CryptoUtils.EncryptString(custom.Type.ToString(), this.vault, conn.WithMasterPassword)
                });
            }
            newitem.custom = tmp.ToArray();
            return newitem;
        }

        private void MapFromItem()
        {
            //Id = item.id;
            Name = listitem.name;
            Url = listitem.url;
            Login = listitem.login;
            Color = listitem.color;
            Description = listitem.description;
            Tags = listitem.tags;
        }


        private async Task Update(PasswordSingleItem item)
        {
            var result = await conn.Put<PasswordSingleItem, PasswordSingleItem>($"passwords/{item.id}", item);
            if (result.status == "success")
            {
                listitem = result.data;
                MapFromItem();
            }
        }

        private async Task Create(PasswordSingleItem item)
        {
            var result = await conn.Post<PasswordSingleItem, PasswordSingleItem>("passwords", item);
            if (result.status == "success")
            {
                listitem = result.data;
                MapFromItem();
            }
        }

        private async Task LoadVault()
        {
            //we also (might) need the vault for decryption
            var vsi = await this.conn.Get<VaultSingleItem>($"vaults/{listitem.vaultId}");
            vault = new Vault(conn, vsi);
        }
    }
}
