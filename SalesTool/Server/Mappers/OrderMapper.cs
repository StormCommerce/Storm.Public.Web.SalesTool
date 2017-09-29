using Enferno.Public.Web.SalesTool.Server.Models;
using Enferno.StormApiClient;
using Enferno.StormApiClient.Orders;
using Enferno.Web.StormUtils;
using System;
using System.Collections.Generic;

namespace Enferno.Public.Web.SalesTool.Server.Mappers
{
    public class OrderMapper
    {
        private readonly bool _showPricesIncVat;
        private readonly string _cultureCode;

        public OrderMapper(bool showPricesIncVat, string cultureCode)
        {
            _showPricesIncVat = showPricesIncVat;
            _cultureCode = cultureCode;
        }

        public OrderModel MapToOrderModel(Order order)
        {
            if (order == null)
                return null;
            var sellToCustomer = new Customer();
            if (order.SellTo.Customer != null)
                sellToCustomer = order.SellTo.Customer;
            var sellToCompany = new Company();
            if (order.SellTo.Company != null)
                sellToCompany = order.SellTo.Company;
            var sellToAddress = new Address();
            if (order.SellTo.Address != null)
                sellToAddress = order.SellTo.Address;
            var shipToCustomer = new Customer();
            if (order.ShipTo.Customer != null)
                shipToCustomer = order.ShipTo.Customer;
            var shipToCompany = new Company();
            if (order.ShipTo.Company != null)
                shipToCompany = order.ShipTo.Company;
            var shipToAddress = new Address();
            if (order.ShipTo.Address != null)
                shipToAddress = order.ShipTo.Address;
            OrderStatus status;
            Enum.TryParse(order.Status, out status);
            var model = new OrderModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNo,
                OrderDate = order.OrderDate,
                Buyer = new CustomerItemModel
                {
                    Id = sellToCustomer.Id.GetValueOrDefault(),
                    Name = sellToCustomer.FirstName + " " + sellToCustomer.LastName,
                    Email = sellToCustomer.Email,
                    Phone = !String.IsNullOrEmpty(sellToCustomer.CellPhone) ? sellToCustomer.CellPhone : sellToCustomer.Phone,
                    CompanyId = sellToCompany.Id.GetValueOrDefault(),
                    CompanyName = sellToCompany.Name,
                },
                BuyerAddress = new AddressModel
                {
                    Address1 = sellToAddress.Line1,
                    Address2 = sellToAddress.Line2,
                    CareOf = sellToAddress.CareOf,
                    Zip = sellToAddress.ZipCode,
                    City = sellToAddress.City,
                    Country = sellToAddress.Country,
                },
                ShipTo = new CustomerItemModel
                {
                    Id = shipToCustomer.Id.GetValueOrDefault(),
                    Name = shipToCustomer.FirstName + " " + shipToCustomer.LastName,
                    Email = shipToCustomer.Email,
                    Phone = !String.IsNullOrEmpty(shipToCustomer.CellPhone) ? shipToCustomer.CellPhone : shipToCustomer.Phone,
                    CompanyId = shipToCompany.Id.GetValueOrDefault(),
                    CompanyName = shipToCompany.Name,
                },
                ShipToAddress = new AddressModel
                {
                    Address1 = shipToAddress.Line1,
                    Address2 = shipToAddress.Line2,
                    CareOf = shipToAddress.CareOf,
                    Zip = shipToAddress.ZipCode,
                    City = shipToAddress.City,
                    Country = shipToAddress.Country,
                },
                DeliveryMethodName = order.DeliveryMethod,
                PaymentMethodName = order.PaymentMethod,
                Total = _showPricesIncVat ? order.OrderTotalIncVat : order.OrderTotalExVat,
                TotalInclVat = order.OrderTotalIncVat,
                TotalExclVat = order.OrderTotalExVat,
                Vat = order.OrderTotalIncVat - order.OrderTotalExVat,
                StatusId = (int)status,
                Status = order.Status,
                Rows = new List<OrderRowModel>(),
                DeliveryNotes = new List<DeliveryNoteModel>(),
                NoDeliveryNoteRows = new List<OrderRowModel>(),
            };
            string divisionCode = null;
            order.DeliveryNotes?.ForEach(note =>
            {
                if (!string.IsNullOrEmpty(note.DivisionCode))
                    divisionCode = note.DivisionCode;
                model.DeliveryNotes.Add(MapToDeliveryNoteModel(note));
            });
            if (!string.IsNullOrEmpty(divisionCode))
            {
                var client = new AccessClient();
                var store = client.ApplicationProxy.ListStores2(_cultureCode).Find(s => s.Code == divisionCode);
                model.PickupStore = StoreMapper.MapToStoreModel(store);
            }
            order.Items?.ForEach(orderRow => {
                DeliveryNote deliveryNote = null;
                if (order.DeliveryNotes != null)
                    deliveryNote = order.DeliveryNotes.Find(note => note.Items.Find(noteItem => noteItem.RowNumber == orderRow.RowNumber) != null);
                if(deliveryNote != null)
                {
                    var deliveryNoteItem = deliveryNote.Items.Find(noteItem => noteItem.RowNumber == orderRow.RowNumber);
                    model.DeliveryNotes.Find(note => note.Id == deliveryNote.Id).Rows.Add(MapToOrderRowModel(orderRow, deliveryNoteItem));
                }
                else
                {
                    model.NoDeliveryNoteRows.Add(MapToOrderRowModel(orderRow, null));
                }
                model.Rows.Add(MapToOrderRowModel(orderRow, null));
            });
            return model;
        }

        public OrderItemModel MapToOrderItemModel(Order order)
        {
            OrderStatus status;
            Enum.TryParse(order.Status, out status);
            var model = new OrderItemModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNo,
                OrderDate = order.OrderDate,
                CompanyId = order.SellTo.Company?.Id.GetValueOrDefault() ?? 0,
                CompanyName = order.SellTo.Company?.Name ?? "",
                CustomerId = order.SellTo.Customer?.Id.GetValueOrDefault() ?? 0,
                CustomerName = order.SellTo.Customer?.FirstName + " " + order.SellTo.Customer?.LastName,
                Email = order.SellTo.Customer?.Email ?? "",
                DeliveryMethodName = order.DeliveryMethod,
                Total = _showPricesIncVat ? order.OrderTotalIncVat : order.OrderTotalExVat,
                TotalInclVat = order.OrderTotalIncVat,
                TotalExclVat = order.OrderTotalExVat,
                Vat = order.OrderTotalIncVat - order.OrderTotalExVat,
                StatusId = (int)status,
                Status = order.Status,
            };
            return model;
        }

        public OrderRowModel MapToOrderRowModel(OrderItem item, DeliveryNoteItem noteItem)
        {
            var unitPrice = item.RowAmount / (item.QtyOrdered > 0 ? item.QtyOrdered : 1);
            var vatRate = item.VatRate;
            var contextVatRate = _showPricesIncVat ? vatRate : 1;
            var model = new OrderRowModel
            {
                RowNumber = item.RowNumber,
                PartNo = item.PartNo,
                Name = item.Name,
                Quantity = item.QtyOrdered,
                Price = unitPrice * contextVatRate,
                PriceInclVat = unitPrice * vatRate,
                PriceExclVat = unitPrice,
                Discount = unitPrice + item.UnitDiscount != 0 ? item.UnitDiscount * 100 / (unitPrice + item.UnitDiscount) : 0,
                VatRate = item.VatRate ,
                Total = item.RowAmount * contextVatRate,
                TotalInclVat = item.RowAmount * vatRate,
                TotalExclVat = item.RowAmount,
            };
            if (noteItem != null)
            {
                OrderStatus status;
                Enum.TryParse(noteItem.Status, out status);
                model.Delivered = noteItem.QtyDelivered;
                model.StatusId = (int)status;
                model.Status = noteItem.Status;
            }
            return model;
        }

        public DeliveryNoteModel MapToDeliveryNoteModel(DeliveryNote note)
        {
            OrderStatus status;
            Enum.TryParse(note.Status, out status);
            var model = new DeliveryNoteModel
            {
                Id = note.Id.GetValueOrDefault(),
                OrderNumber = note.OrderNo,
                Number = note.NoteNo,
                ParcelCode = note.Packages.Count > 0 ? note.Packages[0].ParcelNumber : string.Empty,
                StatusId = (int)status,
                Status = note.Status,
                Rows = new List<OrderRowModel>(),
            };
            return model;
        }
    }
}