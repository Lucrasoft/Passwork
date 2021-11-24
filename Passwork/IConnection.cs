using Passwork.Models;
using System.Threading.Tasks;

namespace Passwork
{
    /// <summary>
    /// Provides the REST calls and takes care of the needed headers.
    /// </summary>
    internal interface IConnection
    {

        string UserName { get; }
        string UserMail { get; }
        string LastError { get; }

        /// <summary>
        /// Indicates that the connection was established with a masterpassword.
        /// </summary>
        bool WithMasterPassword { get; }

        string MasterPassword { get; }

        Task<TResponse> Get<TResponse>(string url);
        Task<Response<TResponse>> Post<T, TResponse>(string url, T payload);
        Task<Response<TResponse>> Put<T, TResponse>(string url, T payload);
        Task<Response<TResponse>> Delete<TResponse>(string url);
      
    }
}
