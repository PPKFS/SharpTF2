using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2
{
    public abstract class ProfileRequest : IRequest
    {
        public String ProfileID { get; set; }

        public abstract String GetJSON();

        public void SetProfileID(string ID)
        {
            throw new NotImplementedException();
        }
    }
}
