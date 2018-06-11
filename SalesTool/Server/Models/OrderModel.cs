using System;
using System.Collections.Generic;

namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class OrderModel
    {
        public long Id;
        public string OrderNumber;
        public int? BasketId;
        public DateTime OrderDate;
        public CustomerItemModel Buyer;
        public AddressModel BuyerAddress;
        public CustomerItemModel ShipTo;
        public AddressModel ShipToAddress;
        public string DeliveryMethodName;
        public StoreModel PickupStore;
        public string PaymentMethodName;
        public decimal Total;
        public decimal TotalInclVat;
        public decimal TotalExclVat;
        public decimal Vat;
        public int StatusId;
        public string Status;
        
        public List<OrderRowModel> Rows;
        public List<DeliveryNoteModel> DeliveryNotes;
        public List<OrderRowModel> NoDeliveryNoteRows;
    }
}