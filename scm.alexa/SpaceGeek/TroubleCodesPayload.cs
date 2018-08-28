using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Model
{
    public class TroubleCodesPayload
    {
        public string CarId { get; set; }
        public string ErrorCode { get; set; }
        public string Description { get; set; }
        public long PayloadTimestamp { get; set; }
    }
}
