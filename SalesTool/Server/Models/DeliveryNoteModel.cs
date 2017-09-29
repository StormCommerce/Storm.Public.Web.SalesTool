using System.Collections.Generic;

namespace Enferno.Public.Web.SalesTool.Server.Models
{
    public class DeliveryNoteModel
    {
        public int Id;
        public string Number;
        public string OrderNumber;
        public string ParcelCode;
        public int StatusId;
        public string Status;

        public List<OrderRowModel> Rows;
    }
}