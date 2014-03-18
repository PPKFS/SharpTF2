using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2.Requests
{
    public abstract class ProfileRequest : BaseRequest
    {
        public String ProfileID { get; set; }

        protected void CheckForProfileID()
        {
            if (ProfileID == null)
                throw new InvalidOperationException("No profile ID provided.");
        }
    }
}
