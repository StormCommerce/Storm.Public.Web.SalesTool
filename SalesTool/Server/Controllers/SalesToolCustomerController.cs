using Enferno.Public.Web.SalesTool.Server.Mappers;
using Enferno.Public.Web.SalesTool.Server.Models;
using Enferno.Web.StormUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Enferno.Public.Web.SalesTool.Server.Controllers
{
    [RoutePrefix("api/salestool/customer")]
    public class SalesToolCustomerController : SalesToolAbstractController
    {
        [Route("search/{searchstring}")]
        [HttpGet]
        public List<CustomerItemModel> Search(string searchString)
        {
            if (!Config.IsActive || searchString == null || searchString.Length < 2)
                return new List<CustomerItemModel>();
            try { 
                var customers = Client.CustomerProxy.SearchCustomer(searchString, null, null, "10", "None", StormContext.CultureCode);
                return customers.Select(CustomerMapper.MapToCustomerItemModel).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex, searchString);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [Route("set/{value}")]
        [HttpPut]
        public SalesToolModel Set(string value)
        {
            int id;
            if(Config.IsActive && int.TryParse(value, out id)) { 
                StormContext.CustomerId = id;
                TransferBasket();
            }
            return new SalesToolController().GetSalesTool();
        }

        [Route("clear")]
        [HttpPut]
        public SalesToolModel Clear()
        {
            if (Config.IsActive)
            {
                StormContext.CustomerId = null;
                TransferBasket();
            }
            return new SalesToolController().GetSalesTool();
        }

        private void TransferBasket()
        {
            if (StormContext.BasketId.HasValue)
            {
                try { 
                    StormApiClient.Customers.Customer customer = null;
                    if (StormContext.CustomerId.HasValue)
                        customer = Client.CustomerProxy.GetCustomer(StormContext.CustomerId.Value, StormContext.CultureCode);
                    if(customer != null)
                        Client.ShoppingProxy.UpdateBuyer(StormContext.BasketId.Value, customer, StormContext.AccountId.GetValueOrDefault(1), StormContext.PriceListIdSeed, StormContext.CultureCode, StormContext.CurrencyId.ToString());
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
                }
            }

        }
    }
}
