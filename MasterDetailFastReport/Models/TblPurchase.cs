namespace MasterDetailFastReport.Models
{
    public class TblPurchase
    {
        public int ID { get; set; }
        public string InvoiceNO { get; set; }
        public DateTime? CreateDate { get; set; }
        //public int TblPurchaseDetailID { get; set; }
       // public virtual TblPurchaseDetail TblPurchaseDetail { get; set; }
    }
}
