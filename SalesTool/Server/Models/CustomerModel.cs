using System.Collections.Generic;

namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class CustomerModel
    {
        public int Id;
        public string Name;
        public string Email;
        public string Phone;
        public string CellPhone;
        public AddressModel InvoiceAddress;
        public List<AddressModel> DeliveryAddresses;
    }
}