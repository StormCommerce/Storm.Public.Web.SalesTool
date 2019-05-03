using Enferno.Public.Web.SalesTool.Server.Mappers;
using Enferno.Public.Web.SalesTool.Server.Models;
using Enferno.Web.StormUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Enferno.Public.InversionOfControl;

namespace Enferno.Public.Web.SalesTool.Server.Controllers
{
    [RoutePrefix("api/salestool/order")]
    public class SalesToolOrderController : SalesToolAbstractController
    {
        private readonly OrderMapper _orderMapper;

        public SalesToolOrderController()
        {
            _orderMapper = new OrderMapper(StormContext.ShowPricesIncVat.GetValueOrDefault(), StormContext.CultureCode);
        }

        [Route("get/{orderNumber}")]
        [HttpGet]
        public OrderModel Get(string orderNumber)
        {
            if (!Config.IsActive)
                return new OrderModel();
            try
            {
                var item = Client.OrderProxy.GetOrderByNo(orderNumber, StormContext.CultureCode);
                if (item == null)
                    return null;
                return _orderMapper.MapToOrderModel(item);
            }
            catch (Exception ex)
            {
                LogError(ex, orderNumber);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [Route("list")]
        [HttpGet]
        public List<OrderItemModel> List()
        {
            if (!Config.IsActive)
                return new List<OrderItemModel>();

            if (!StormContext.CompanyId.HasValue && !StormContext.CustomerId.HasValue)
                return new List<OrderItemModel>();
            try
            {
                var list = Client.OrderProxy.ListOrders3(
                    StormContext.CompanyId.HasValue ? StormContext.CompanyId.ToString() : null,
                    StormContext.CustomerId.HasValue ? StormContext.CustomerId.ToString() : null,
                    null, null, null, null, "0", "1000", null, StormContext.CultureCode);
                return list.Items
                    .OrderByDescending(item => item.OrderDate)
                    .Select(_orderMapper.MapToOrderItemModel).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [Route("liststore")]
        [HttpGet]
        public List<OrderItemModel> ListStore()
        {
            return ListStore(null);
        } 

        [Route("liststore/{statusSeed}")]
        [HttpGet]
        public List<OrderItemModel> ListStore(string statusSeed)
        {
            var list = new List<OrderItemModel>();
            if (!StormContext.DivisionId.HasValue)
                return list;
            var store = Client.ApplicationProxy.GetStore(StormContext.DivisionId.Value, null, null, StormContext.CultureCode);
            if (string.IsNullOrEmpty(store?.Code))
                return list;
            try
            {
                var result = Client.OrderProxy.ListOrders3(null, null, statusSeed, null, null, null, "0", "1000", store.Code, StormContext.CultureCode);
                return result.Items
                    .OrderByDescending(item => item.OrderDate)
                    .Select(_orderMapper.MapToOrderItemModel)
                    .ToList();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [Route("search/{searchstring}")]
        [HttpGet]
        public OrderModel Search(string searchstring)
        {
            if (!Config.IsActive)
                return new OrderModel();

            try
            {
                var order = Client.OrderProxy.GetOrderByNo(searchstring, StormContext.CultureCode) 
                            ?? Client.OrderProxy.SearchOrder(searchstring, StormContext.CultureCode);
                return _orderMapper.MapToOrderModel(order);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Bad Request")
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Order not found") });
                LogError(ex, searchstring);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("Unknown error") });
            }
        }

        [Route("updatedeliverynotestatus")]
        [HttpPut]
        public OrderModel UpdateDeliveryNoteStatus(DeliveryNoteModel deliveryNote)
        {
            if (!Config.IsActive)
                return new OrderModel();

            try
            {
                UpdateOrderStatus(deliveryNote.OrderNumber, deliveryNote.Id, deliveryNote.Status);
                var order = Client.OrderProxy.GetOrderByNo(deliveryNote.OrderNumber, StormContext.CultureCode);
                if(deliveryNote.Status == "ReadyForPickup" || deliveryNote.Status == "Reserved") { 
                    SendOrderNotification(order.OrderNo, order.SellTo?.Customer?.Id);
                }
                return _orderMapper.MapToOrderModel(order);
            }
            catch (Exception ex)
            {
                LogError(ex, deliveryNote.OrderNumber, deliveryNote.Id, deliveryNote.Status);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        private void UpdateOrderStatus(string orderNumber, int? deliveryNoteId, string status)
        {
            var order = Client.OrderProxy.GetOrderByNo(orderNumber, StormContext.CultureCode);
            if (order.DeliveryNotes == null) return;

            foreach (var package in order.DeliveryNotes
                .Where(note => (!deliveryNoteId.HasValue || note.Id == deliveryNoteId) 
                    && note.Packages != null)
                .SelectMany(note => note.Packages))
            {
                package.Status = status;
                Client.OrderProxy.UpdateDeliveryNotePackage(package, StormContext.CultureCode);
            }
        }

        [Route("print/{orderNumber}")]
        [HttpGet]
        public string PrintDeliveryNote(string orderNumber)
        {
            if (!Config.IsActive)
                return null;
            var config = (SalesToolSection)System.Configuration.ConfigurationManager.GetSection("enferno.salesTool");
            var path = config.DeliveryNotePrintXsltPath;
            var xml = new XmlHelper();
            if (string.IsNullOrEmpty(path)) return null;
            var orderXml = xml.SerializeToXmlDocument(Get(orderNumber));
            return xml.TransformXml(orderXml, path);
        }

        [Route("notifycheckedorder")]
        [HttpPut]
        public void NotifyCheckedOrder([FromBody]OrderModel order)
        {
            if (!Config.IsActive)
                return;
            SendOrderNotification(order.OrderNumber, order.Buyer.Id);
        }

        [Route("notifycheckedorders")]
        [HttpPut]
        public List<OrderItemModel> NotifyCheckedOrders([FromBody]List<OrderItemModel> orders)
        {
            if (!Config.IsActive)
                return orders;
            foreach (var item in orders.Where(item => item.Checked))
            {
                item.Checked = false;
                SendOrderNotification(item.OrderNumber, item.CustomerId);
            }
            return orders;
        }

        private void SendOrderNotification(string orderNumber, int? customerId)
        {
            if (string.IsNullOrEmpty(orderNumber))
                return;
            var action = IoC.IsRegistered<ISalesToolAction>() ? IoC.Resolve<ISalesToolAction>() : new SalesToolAction();
            StormApiClient.Customers.Customer customer = null;
            if (customerId.HasValue)
                customer = Client.CustomerProxy.GetCustomer(customerId.Value, null);
            var order = Client.OrderProxy.GetOrderByNo(orderNumber, null);
            if(order.Status == "ReadyForReservation" || order.Status == "Reserved")
                action.SendReservationNotification(customer, order);
            else
                action.SendPickupNotification(customer, order);
        }
    }
}
