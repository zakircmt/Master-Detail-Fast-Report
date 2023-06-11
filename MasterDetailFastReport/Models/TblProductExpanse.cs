namespace MasterDetailFastReport.Models
{
    public class TblProductExpanse
    {
        public int ID { get; set; }
        public string ExpanseName { get; set; }
        public decimal ExpanseCost { get; set; }
        public int TblPurchaseDetailID { get; set; }
        public TblPurchaseDetail TblPurchaseDetail { get; set; }
    }
}
