using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MockHttpClient.Content
{
    /// <summary>
    /// Represents Xml content.
    /// </summary>
    /// <seealso cref="System.Net.Http.StringContent" />
    public class XmlContent : StringContent
    {
        private static readonly string DefaultMediaType = "application/xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlContent"/> class.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="mediaType">The media type. Defaults to application/xml.</param>
        public XmlContent(object obj, string mediaType = null) 
            : base(ToXmlString(obj), null, mediaType ?? DefaultMediaType)
        {

        }

        private static string ToXmlString(object input)
        {
            if (input == null)
                return "";

            using (var writer = new StringWriter())
            {
                new XmlSerializer(input.GetType()).Serialize(writer, input);
                return writer.ToString();
            }
        }
    }
}
