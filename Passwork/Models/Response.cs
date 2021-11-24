using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork.Models
{
 
    /// <summary>
    /// Default response model of Passwork API response
    /// </summary>
    internal class Response<T>
    {
        public string status { get; set; }
        public T data { get; set; }
        public string code { get; set; }
    }

}
