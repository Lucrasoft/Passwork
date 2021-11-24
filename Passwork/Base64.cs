using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passwork
{
    /// <summary>
    /// String To Base64 conversion, based on UTF8.
    /// </summary>
    public static class Base64
    {
        public static string Decode(string base64EncodedData)
        {
            if (base64EncodedData == null) return null;
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Encode(string plainText)
        {
            if (plainText == null) return null;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
