using System;
using Enferno.Public.Web.SalesTool.Server.Mappers;
using Enferno.StormApiClient.Customers;
using Enferno.StormApiClient.Orders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Address = Enferno.StormApiClient.Orders.Address;
using Company = Enferno.StormApiClient.Orders.Company;
using Customer = Enferno.StormApiClient.Orders.Customer;

namespace SalesTool.Tests.Server.Mappers
{
    [TestClass]
    public class OrderMapperTest
    {
        [TestMethod]
        public void MapOrderTest()
        {
            // Arrange
            var customer = new CustomerData
            {
                Customer = new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Testington",
                    Email = "test@gmail.com",
                    CellPhone = "010-123456"
                },
                Company = new Company
                {
                    Id = 2,
                    Name = "Acme Inc."
                },
                Address = new Address
                {
                    Line1 = "Street 1",
                    ZipCode = "12345",
                    City = "Demo city"
                }
            };
            var entity = new OrderListItem
            {
                Id = 1,
                OrderNo = "123456",
                OrderDate = DateTime.Now,
                SellTo = customer,
                BillTo = customer,
                ShipTo = customer,
                DeliveryMethod = "Signed, sealed, delivered",
                OrderTotalExVat = 800M,
                OrderTotalIncVat = 1000M,
                Status = "Confirmed"
            };
            var orderMapper = new OrderMapper(true, "sv");

            // Act
            var order = orderMapper.MapToOrderItemModel(entity);

            // Assert
            Assert.AreEqual(entity.Id, order.Id);
            Assert.AreEqual(entity.OrderNo, order.OrderNumber);
            Assert.AreEqual(entity.SellTo.Company.Id, order.CompanyId);
            Assert.AreEqual(entity.OrderTotalIncVat, order.Total);
            Assert.AreEqual(2, order.StatusId);
        }
    }
}
