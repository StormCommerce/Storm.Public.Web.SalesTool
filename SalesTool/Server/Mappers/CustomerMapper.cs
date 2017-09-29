using Enferno.Public.Web.SalesTool.Server.Models;
using Enferno.StormApiClient.Customers;

namespace Enferno.Public.Web.SalesTool.Server.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerItemModel MapToCustomerItemModel(Customer customer)
        {
            if (customer == null)
                return null;
            var model = new CustomerItemModel
            {
                Id = customer.Id.GetValueOrDefault(),
                Name = customer.FirstName + " " + customer.LastName,
                Email = customer.Email,
                CompanyId = customer.Companies != null && customer.Companies.Count > 0 ? customer.Companies[0].Id.GetValueOrDefault() : 0,
                CompanyName = customer.Companies != null && customer.Companies.Count > 0 ? customer.Companies[0].Name : string.Empty
            };
            return model;
        }
    }
}