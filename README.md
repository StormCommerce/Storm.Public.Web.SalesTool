# Storm Public Web SalesTool
Enferno UI component for sales staff to impersonate customers and place orders and quotations. This is a guide to implement the SalesTool into your STORM site.

## Add NuGet Package
Use "Manage NuGet packages" from solution explorer or add from Package Manager Console:
```
PM> Install-Package Enferno.Public.Web.SalesTool
```
## Insert SalesTool UI, script and css
### WebForms master page
At the top of the master page right below form tag, insert a tag to render the UI of the SalesTool. It will only render if it is allowed by logged in user role.
```html
<head>
    <%= Enferno.Public.Web.SalesTool.Config.IsActive ? "<link href=\"/Content/salestool.min.css\" rel=\"stylesheet\"/>" : "" %>
</head>
<body>
    <%= Enferno.Public.Web.SalesTool.UI.Render() %>
    <form id="F" runat="server" action="#">
        ...
    </form>
    <%= Enferno.Public.Web.SalesTool.Config.IsActive ? "<script src=\"/Scripts/jquery-1.10.0.js\"></script><script src=\"/Scripts/knockout-3.2.0.js\"></script><script src=\"/Scripts/knockout.mapping-latest.js\"></script><script src=\"/Scripts/salestool.min.js\"></script>" : "" %>
</body>
```
### MVC template
At the top of the template right below body tag, insert a tag to render the UI of the SalesTool. It will only render if it is allowed by logged in user role.
```html
<head>
    @Html.Raw(Enferno.Public.Web.SalesTool.Config.IsActive ? "<link href=\"/Content/salestool.min.css\" rel=\"stylesheet\"/>" : "")
</head>
<body>
    @Html.Raw(Enferno.Public.Web.SalesTool.UI.Render())
    @Html.Raw(Enferno.Public.Web.SalesTool.Config.IsActive ? "<script src=\"/Scripts/jquery-1.10.0.js\"></script><script src=\"/Scripts/knockout-3.2.0.js\"></script><script src=\"/Scripts/knockout.mapping-latest.js\"></script><script src=\"/Scripts/salestool.min.js\"></script>" : "")
</body>
```
It is easy to change the appearance of the SalesTool by switching out the css and using your own modified version. All styles are configurable in this way.

## Activate WebApi and enabling attribute routing
In global.asax.cs you add WebApi initializer.
```csharp
        void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
```
In App_Start folder, add a new static WebApiConfig class. If you already initialize WebAPI, then you just have to enable attribute routing with config.MapHttpAttributeRoutes().
```csharp
using System.Web.Http;
namespace Client.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Attribute routing.
            config.MapHttpAttributeRoutes();

            // Convention-based routing.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
```
## ISalesToolAction
This interface can be implemented to send order notifications to customers. 
BREAKING CHANGE IN SalesTool 1.2.0: SendNotification has been renamed to SendPickupNotification.

This is an example: 
```csharp
public class MySalesToolAction : Enferno.Public.Web.SalesTool.ISalesToolAction
    {
        public void SendPickupNotification(Customer customer, Order order)
        {
            // Your implementation of sending pick up notification either by mail or sms or any other way.
            // You get a full customer and order object from Storm API that you can pick information for your mail/sms template
        }

        public void SendReservationNotification(Customer customer, Order order)
        {
            // Your implementation of sending reservation notification either by mail or sms or any other way
            // You get a full customer and order object from Storm API that you can pick information for your mail/sms template
        }
    }
```
And then you need to configure Unity to resolve ISalesToolAction to your class.
```xml
  <unity xmlns="http://schemas.microsoft.com/practices/2010/unity">
    <assembly name="Enferno.Public.Web.SalesTool" />
    <namespace name="Enferno.Public.Web.SalesTool" />
    <container>
      <register type="ISalesToolAction" mapTo="MySalesToolAction">
        <constructor />
      </register>
    </container>
  </unity>
```
## Configuration
Enferno SalesTool can be configured to activate or deactivate certain functions and you can also set templates for mail or print.
```xml
  <configSections>
    <section name="enferno.salesTool" type="Enferno.Public.Web.SalesTool.SalesToolSection, Enferno.Public.Web.SalesTool" />
  </configSections>
  <enferno.salesTool useOrders="true" useStorePickup="true" useStoreReservation="true" notifyOrder="true" deliveryNotePrintXsltPath="~/App_Data/deliveryNotePrint.xslt" />
```
### useOrders
Set this to true and the sales tool will show the order bar where you can search for orders and list previous orders for a customer.
###useStorePickup
Set this to true if you want your stores to be able to check in goods that is to be picked up at store and then check out to customer when they come to pick up their delivery.
### useStoreReservation
Set this to true if you want your stores to be able to handle reservations.
### notifyOrder
Set this to true if you have implemented ISalesToolAction.SendPickupNotification and/or SendReservationNotification to send notifications to customers.
### popupNewReservations
Shows a popup when new reservations arrive. Default is false.
### deliveryNotePrintXsltPath
An application relative path to your xslt template for printing an order. The NuGet package contains an example of the xml that is to be transformed and an xslt that shows how this can be built. Just add your design to the xslt to be able to print delivery notes. If you leave this empty or omit the configuration then the print button will be removed.
### Show prices with VAT included
SalesTool uses StormContext.showPricesIncVat to determine if prices should be shown with or without VAT. If you want to set this for all new sessions you can add this to the stormSettings configuration section. Just add showPricesIncVat="true" as an attribute to stormSettings and prices will default to be shown with VAT included.

## Add cache settings
When you use Enferno.WebApiClient you can activate application caching of API calls to increase performance. Enferno SalesTool use your WebApiClient to access the API and will also use your AccessClient.cache,xml that is located in the App_Data folder. Add these lines if they're not already present:
```xml
  <Item name="GetApplication"/>
  <Item name="GetCustomer" redirectformat="Customer{0}" propertypath="Id"/>
  <Item name="GetCustomerByAccountId"/>
  <Item name="GetStore"/>
  <Item name="ListStores2"/>
  <Item name="SearchCustomer" />
  <Item name="UpdateBuyer" duration="0" redirectformat="Basket{0};Checkout{0}" propertypath="Basket.Id"/>
```
The SaleTool also uses GetOrderByNo, ListOrders and SearchOrder but these are better not cached so updates will be shown right away.

That's about it. Enferno needs to add a new Sales role in STORM with a sales authorization. And you need a login page for sales personel to log in to the site. If a logged in user has sales authorization set in its role, then the sales tool will be rendered.
