using System.Collections.Generic;

namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class CompanyModel
    {
        public int Id;
        public string Name;
        public string Phone;
        public string OrganizationNumber;
        public AddressModel InvoiceAddress;
        public List<AddressModel> DeliveryAddresses;
    }
}