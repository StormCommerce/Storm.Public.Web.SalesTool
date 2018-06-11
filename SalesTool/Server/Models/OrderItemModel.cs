using System;

namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class OrderItemModel
    {
        public long Id;
        public string OrderNumber;
        public int? BasketId;
        public DateTime OrderDate;
        public int CompanyId;
        public string CompanyName;
        public int CustomerId;
        public string CustomerName;
        public string Email;
        public string DeliveryMethodName;
        public decimal Total;
        public decimal TotalInclVat;
        public decimal TotalExclVat;
        public decimal Vat;
        public int StatusId;
        public string Status;
        public bool Checked;
    }
}