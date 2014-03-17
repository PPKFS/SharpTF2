using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTF2
{
    public abstract class APIKeyRequest : ProfileRequest
    {
        public String APIKey { get; set; }
    }
}
