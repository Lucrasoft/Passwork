using System.Threading.Tasks;

namespace Passwork
{
    public interface IFolder
    {
        string Id { get; }
        string Name { get; }

        int AmountFolders { get; }
        int AmountPasswords { get; }

        Task Refresh();

        Task<IFolder> AddFolder(string name);

        Task<IFolder[]> GetFolders();

        Task<IFolder> GetFolderByName(string name);

        IPasswordNew CreatePassword();

        Task<IPassword[]> GetPasswords();

        Task<bool> Delete();

        Task<IFolder> Rename(string name);
       
    }
}
