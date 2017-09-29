using Enferno.StormApiClient.Orders;
using Customer = Enferno.StormApiClient.Customers.Customer;

namespace Enferno.Public.Web.SalesTool
{
    public interface ISalesToolAction
    {
        void SendPickupNotification(Customer customer, Order order);
        void SendReservationNotification(Customer customer, Order order);
    }
    public class SalesToolAction : ISalesToolAction
    {
        public void SendPickupNotification(Customer customer, Order order)
        {
            // Do nothing
        }
        
        public void SendReservationNotification(Customer customer, Order order)
        {
            // Do nothing
        }
    }
}