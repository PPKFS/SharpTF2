using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTF2
{
    public interface IRequest
    {
        void SetProfileID(String ID);

        String GetJSON();
    }
}
