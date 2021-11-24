using Passwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{
    

    public interface IPasswordQuery
    {
        IPasswordQuery WhereColor(int color);
        IPasswordQuery WhereTag(string tag);
        IPasswordQuery WhereFieldsLike(string name);
        Task<IPassword[]> Get();
    }


    class PasswordQuery : IPasswordQuery
    {

        private readonly IConnection conn;
        private readonly string vaultId;

        private string search;
        private readonly List<int> colors;
        private readonly List<string> tags;

        public PasswordQuery(IConnection conn, string vaultId = "")
        {
            this.conn = conn;
            this.vaultId = vaultId;
            this.colors = new List<int>();
            this.tags = new List<string>();
        }

        public IPasswordQuery WhereColor(int color)
        {
            this.colors.Add(color);
            return this;
        }
        public IPasswordQuery WhereTag(string tag)
        {
            this.tags.Add(tag);
            return this;
        }

        public IPasswordQuery WhereFieldsLike(string text)
        {
            //TODO : Passwork needs text to be at least 2 chars.
            this.search = text;
            return this;
        }

        public async Task<IPassword[]> Get()
        {
            var requestObj = new SearchRequest();
            if (colors.Count>0) { requestObj.colors = colors.ToArray(); }
            if (tags.Count>0) { requestObj.tags = tags.ToArray(); }
            if (!string.IsNullOrEmpty(vaultId)) { requestObj.vaultId = vaultId; }
            if (!string.IsNullOrEmpty(search)) { requestObj.query = search; }

            //validate the requestObj...

            var response = await conn.Post<SearchRequest, PasswordListItem[]>("passwords/search", requestObj);
            if (response.status == "success")
            {
                var result = new List<IPassword>();
                foreach (var item in response.data)
                {
                    result.Add(new Password(conn, item));
                }
                return result.ToArray();
            }
            return Array.Empty<IPassword>();
        }


    }
}
