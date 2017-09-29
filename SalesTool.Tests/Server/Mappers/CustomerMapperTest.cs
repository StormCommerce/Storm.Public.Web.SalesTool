using System;
using Enferno.Public.Web.SalesTool.Server.Mappers;
using Enferno.StormApiClient.Customers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesTool.Tests.Server.Mappers
{
    [TestClass]
    public class CustomerMapperTest
    {
        [TestMethod]
        public void PrivateMapperTest()
        {
            // Arrange
            var entity = new Customer
            {
                Id = 1,
                FirstName = "Test1",
                LastName = "Testington",
                Email = "test1@gmail.com"
            };

            // Act
            var customer = CustomerMapper.MapToCustomerItemModel(entity);

            // Assert
            Assert.AreEqual(customer.Id, entity.Id);
            Assert.AreEqual(customer.Name, entity.FirstName + " " + entity.LastName);
            Assert.AreEqual(customer.Email, entity.Email);
            Assert.AreEqual(customer.CompanyId, 0);
            Assert.AreEqual(customer.CompanyName, string.Empty);
        }

        [TestMethod]
        public void CompanyMapperTest()
        {
            // Arrange
            const string companyName = "Acme Inc.";
            var entity = new Customer
            {
                Id = 2,
                FirstName = "Test2",
                LastName = "Testington",
                Email = "test2@gmail.com",
                Companies = new CompanyList
                {
                    new Company
                    {
                        Id = 3,
                        Name = companyName
                    }
                }
            };

            // Act
            var customer = CustomerMapper.MapToCustomerItemModel(entity);

            // Assert
            Assert.AreEqual(customer.Id, entity.Id);
            Assert.AreEqual(customer.Name, entity.FirstName + " " + entity.LastName);
            Assert.AreEqual(customer.Email, entity.Email);
            Assert.AreEqual(customer.CompanyId, 3);
            Assert.AreEqual(customer.CompanyName, companyName);
        }
    }
}
