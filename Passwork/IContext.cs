using System.Threading.Tasks;

namespace Passwork
{
    public interface IContext
    {
        string LastError { get; }

        /// <summary>
        /// Add a vault to this connection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isPrivate"></param>
        /// <returns></returns>
        Task<IVault> AddVault(string name, bool isPrivate);
        
        /// <summary>
        /// Get all vaults in the connection.
        /// </summary>
        /// <returns></returns>
        Task<IVault[]> GetVaults();


        Task<IVault> GetVaultByName(string name);
       
        /// <summary>
        /// Gets a vault by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IVault> GetVaultByID(string id);

        Task<IFolder> GetFolderByID(string id);
        Task<IPassword> GetPasswordByID(string id);
        Task<IPassword[]> GetPasswordsRecent();
        Task<IPassword[]> GetPasswordsFavorite();

        /// <summary>
        /// Get all colors used
        /// </summary>
        /// <returns></returns>
        Task<int[]> GetColors();

        /// <summary>
        /// Get all tags used 
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetTags();

        /// <summary>
        /// Get acces to the query builder.
        /// </summary>
        /// <returns></returns>
        IPasswordQuery Query();

    }
}
