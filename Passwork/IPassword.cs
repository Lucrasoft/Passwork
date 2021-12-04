using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passwork
{

    public interface IPasswordNew
    {
        string Id { get; }
        string VaultId { get; }
        string Name { get; set; }
        string Url { get; set; }
        string Pass { get; set; }
        string Login { get; set; }
        string Description { get; set; }
        string[] Tags { get; set; }
        int? Color { get; set; }
        List<CustomField> CustomRecords { get; set; }

        /// <summary>
        /// Saves the password. Returns a continuation object if succesfull
        /// </summary>
        /// <returns>an IPassword object is successfull, otherwise null</returns>
        Task<IPassword> Save();

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

        Task<IAttachment[]> GetAttachments();

        /// <summary>
        /// Add an attachment to this password.
        /// Note: The REST API does not return the ID of the newly created attachment, 
        ///       So the get your net IAttachment, you need to find it in the GetAttachments by name.
        /// </summary>
        /// <param name="name">The Filename</param>
        /// <param name="content">The byte array you wish to attach</param>
        /// <returns>True if the attachment was saved succesfully.</returns>
        Task<bool> AddAttachment(string name, byte[] content);


        /// <summary>
        /// Delete this password
        /// </summary>
        /// <returns></returns>
        Task<bool> Delete();
    }
}
