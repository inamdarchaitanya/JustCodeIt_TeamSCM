using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Model
{
    public class TroubleCodesPayload
    {
        [Key]
        public int TroubleCodeId { get; set; }
        public string CarId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public long PayloadTimestamp { get; set; }
    }
}
