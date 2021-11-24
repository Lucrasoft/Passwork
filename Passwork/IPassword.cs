using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passwork
{

    public interface IPasswordNew
    {
        string Id { get; }
        string Name { get; set; }
        string Url { get; set; }
        string Pass { get; set; }
        string Login { get; set; }
        string Description { get; set; }
        string[] Tags { get; set; }
        int? Color { get; set; }
        List<CustomField> CustomRecords { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task Save();
    }

    public interface IPassword : IPasswordNew
    {
       
        /// <summary>
        /// Loads and unlocks the Password and Custom properties.
        /// </summary>
        /// <returns></returns>
        Task Unlock();

        /// <summary>
        /// Mark/unmark this password as a favorite
        /// </summary>
        /// <param name="IsFavorite"></param>
        /// <returns></returns>
        Task SetFavorite(bool IsFavorite);
    }
}
