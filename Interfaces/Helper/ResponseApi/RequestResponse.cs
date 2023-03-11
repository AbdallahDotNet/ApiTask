using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Helper.ResponseApi
{
    public class RequestResponse
    {
        public string RequestState { get; set; }
        public string RequestMessage { get; set; }
        public object Response { get; set; }
    }
}
