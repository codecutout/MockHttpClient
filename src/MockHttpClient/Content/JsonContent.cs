using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MockHttpClient.Content
{
    /// <summary>
    /// Represents Json content.
    /// </summary>
    /// <seealso cref="System.Net.Http.StringContent" />
    public class JsonContent : StringContent
    {
        private static readonly string DefaultMediaType = "application/json";

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContent"/> class.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="mediaType">The media type. Defaults to application/json</param>
        /// <param name="settings">The json serializer settings.</param>
        public JsonContent(object obj, string mediaType = null, JsonSerializerSettings settings = null) 
            : base(JsonConvert.SerializeObject(obj, settings), null, mediaType ?? DefaultMediaType)
        {

        }
    }
}
