namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class OrderRowModel
    {
        public decimal RowNumber;
        public int DeliveryNoteId;
        public string PartNo;
        public string Name;
        public decimal Price;
        public decimal PriceInclVat;
        public decimal PriceExclVat;
        public decimal VatRate;
        public decimal Discount;
        public decimal Quantity;
        public decimal Total;
        public decimal TotalInclVat;
        public decimal TotalExclVat;
        public decimal Delivered;
        public int StatusId;
        public string Status;
    }
}