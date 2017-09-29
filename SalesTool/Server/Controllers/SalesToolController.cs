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
    [RoutePrefix("api/salestool")]
    public class SalesToolController : SalesToolAbstractController
    {
        [Route("get")]
        [HttpGet]
        public SalesToolModel GetSalesTool()
        {
            var account = Config.SalesPerson;
            if (account == null)
                return null;

            StormApiClient.Applications.Store store = null;
            if (StormContext.DivisionId.HasValue)
            {
                try
                {
                    var storeTemp = Client.ApplicationProxy.GetStore(StormContext.DivisionId.Value, null, null, StormContext.CultureCode);
                    if (storeTemp != null)
                    {
                        store = storeTemp;
                    }
                    else
                    {
                        StormContext.DivisionId = null;
                    }
                }
                catch(Exception ex)
                {
                    LogError(ex);
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
                }
            }
            if(store == null)
                store = new StormApiClient.Applications.Store();

            StormApiClient.Customers.Customer customer = null;
            if (StormContext.CustomerId.HasValue)
            {
                try
                {
                    customer = Client.CustomerProxy.GetCustomer(StormContext.CustomerId.Value, StormContext.CultureCode);
                }
                catch
                {
                    // Silent catch and reset context
                    StormContext.CustomerId = null;
                }
            }
            if (customer == null)
                customer = new StormApiClient.Customers.Customer();

            return new SalesToolModel
            {
                AccountId = account.Account.Id.GetValueOrDefault(),
                AccountName = account.Account.Name,
                AccountImageUrl = "/tomas.jpg",
                StoreId = store.Id,
                StoreName = store.Name,
                CustomerId = customer.Id.GetValueOrDefault(),
                CustomerName = customer.FirstName + (!String.IsNullOrEmpty(customer.FirstName) ? " " : "") + customer.LastName,
                CultureCode = StormContext.CultureCode.Substring(0,2)
            };
        }

        [Route("liststores")]
        [HttpGet]
        public List<IdNameModel> ListStores()
        {
            try { 
                return Client.ApplicationProxy
                    .ListStores2(StormContext.CultureCode)
                    .OrderBy(s => s.Name)
                    .Select(s => new IdNameModel { Id = s.Id, Name = s.Name }).ToList();
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [Route("setstore")]
        [HttpPost]
        public SalesToolModel SetStore([FromBody]string value)
        {
            int id;
            if (int.TryParse(value, out id))
            {
                StormContext.DivisionId = id;
            }
            return GetSalesTool();
        }
    }
}
