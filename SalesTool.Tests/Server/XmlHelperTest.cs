using System;
using System.Xml;
using Enferno.Public.Web.SalesTool.Server;
using Enferno.Public.Web.SalesTool.Server.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesTool.Tests.Server
{
    [TestClass]
    public class XmlHelperTest
    {
        [TestMethod]
        public void SerializeObjectToXmlTest()
        {
            // Arrange
            var xmlHelper = new XmlHelper();
            var model = new OrderModel
            {
                Id = 1,
                OrderNumber = "123456"
            };

            // Act
            var xml = xmlHelper.SerializeToXmlDocument(model);
            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("ns", "http://schemas.enferno.se");
            
            // Assert
            Assert.IsNotNull(xml);
            Assert.AreEqual(model.Id.ToString(), xml.SelectSingleNode("//ns:Id", nsmgr)?.InnerText);
            Assert.AreEqual(model.OrderNumber, xml.SelectSingleNode("//ns:OrderNumber", nsmgr)?.InnerText);
        }
    }
}
