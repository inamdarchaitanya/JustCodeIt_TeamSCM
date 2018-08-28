namespace SCM.Dashboard.Models
{
    public class TroubleCodesPayload
    {
        public int TroubleCodeId { get; set; }
        public string CarId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public long PayloadTimestamp { get; set; }
    }
}
