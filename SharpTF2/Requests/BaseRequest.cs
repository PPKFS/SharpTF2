using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Requests
{
    public abstract class BaseRequest : IRequest
    {
        public String APIKey { get; set; }

        protected void CheckForAPIKey()
        {
            if (APIKey == null)
                throw new InvalidOperationException("No API key provided.");
        }

        public abstract String GetJSON();
    }
}
