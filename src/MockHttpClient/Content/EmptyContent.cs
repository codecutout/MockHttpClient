using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MockHttpClient.Content
{
    /// <summary>
    /// Represents an empty content
    /// </summary>
    /// <seealso cref="System.Net.Http.ByteArrayContent" />
    public class EmptyContent : ByteArrayContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyContent"/> class.
        /// </summary>
        public EmptyContent():base(new byte[0])
        {

        }
    }
}
