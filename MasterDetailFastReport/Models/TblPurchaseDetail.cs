namespace MasterDetailFastReport.Models
{
    public class TblPurchaseDetail
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int QTY { get; set; }
        public int TblPurchaseID { get; set; }
        public virtual TblPurchase TblPurchase { get; set; }
        public virtual List<TblProductExpanse> TblProductExpanse { get; set; }
    }
}
