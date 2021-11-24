using System.Threading.Tasks;

namespace Passwork
{
    public interface IVault
    {
        /// <summary>
        /// ID of the vault. Readonly.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Name of the vault. Readonly
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Create a new password within this vault. 
        /// </summary>
        /// <returns>IPasswordNew</returns>
        IPasswordNew CreatePassword();

        /// <summary>
        /// Get all passwords in this vault.
        /// </summary>
        /// <returns>IPassword[]</returns>
        Task<IPassword[]> GetPasswords();


        /// <summary>
        /// Get all folders in this vault.
        /// </summary>
        /// <returns>IFolder[]</returns>
        Task<IFolder[]> GetFolders();

        /// <summary>
        /// Looks for a foldername within this vault.
        /// Subfolders are not traversed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>IFolder is found, otherwise null</returns>
        Task<IFolder> GetFolderByName(string name);

        /// <summary>
        /// Add a folder to this vault
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<IFolder> AddFolder(string name);


        /// <summary>
        /// Delete this vault
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete();

        //Rename function is not working properly.
        //public Task<IVault> Rename(string name);

        /// <summary>
        /// Get all colors used in this vault.
        /// </summary>
        /// <returns></returns>
        Task<int[]> GetColors();

        /// <summary>
        /// Get all tags used in this vault.
        /// </summary>
        /// <returns></returns>
        Task<string[]> GetTags();

        /// <summary>
        /// Search for passwords within THIS vaults.
        /// </summary>
        /// <returns>A query builder </returns>
        IPasswordQuery Query();

        Task<string> GetMaster();

    }
}
