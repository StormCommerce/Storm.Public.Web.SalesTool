using Enferno.StormApiClient;
using Enferno.StormApiClient.Customers;
using Enferno.Web.StormUtils;
using System.Linq;
using System.Reflection;

namespace Enferno.Public.Web.SalesTool
{
    public enum OrderStatus
    {
        Unknown = 0,
        Allocation = 1,
        Confirmed = 2,
        BackOrder = 3,
        Delivered = 4,
        Invoiced = 5,
        Cancelled = 6,
        CreditControl = 7,
        PartlyDelivered = 8,
        Acknowledged = 9, // Picked up and notified
        ErpConfirmed = 10, // Purchase order is saved in Client ERP
        ReadyForPickup = 11, // Packages is ready for pick up at store
        PickedUp = 12, // Packages is picked up at store
        NotPickedUp = 13, // Packages was returned and not picked up at store
        OnHold = 14, // Purchase order is sent to supplier but will not be processed before it will be released
        WaitingForCancel = 15, // Purchase order cancellation is sent, waiting for confirmation of receipt
        WaitingForOnHoldRemoved = 16, // Remove OnHold flag request is sent to supplier, waiting for confirmation of receipt
        CancelError = 17, // Cancellation attempt failed
        AcknowledgementError = 18, // Purchase order creation attempt failed
        ReadyForReservation = 19, // Items is ready to be reserved in Store
        Reserved = 20 // Items is reserved in Store
    }
    
    public static class Config
    {
        internal const int AllowSales  = 16;
        internal const int AllowSalesAdmin = 18;
        internal static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static bool IsActive => SalesPerson != null;

        internal static Customer SalesPerson
        {
            get
            {
                if (!StormContext.AccountId.HasValue) return null;
                var client = new AccessClient();
                try {
                    var customer = client.CustomerProxy.GetCustomerByAccountId(StormContext.AccountId.Value, StormContext.CultureCode);
                    if (customer != null && customer.Account.Authorizations.Any(a => a.Id == (int)AllowSales || a.Id == (int)AllowSalesAdmin))
                        return customer;
                } catch
                {
                    // Silent catch. Treat as no sales person
                }
                return null;
            }
        }
    }

    // ReSharper disable once InconsistentNaming
    public static class UI
    {
        public static string Render()
        {
            if (!Config.IsActive)
                return null;
            var config = (SalesToolSection) System.Configuration.ConfigurationManager.GetSection("enferno.salesTool");
            
            var html = @"
            <link href='https://fonts.googleapis.com/css?family=Lato:300,700' rel='stylesheet' type='text/css'>
            <div id=""sales-tool"" style=""display: none"" data-bind=""visible: isLoaded"">
            <div id=""sales-tool-backdrop"" data-bind=""visible: isLoading, click: preventBubble""></div>
            <div id=""sales-tool-loader"" data-bind=""visible: isLoading""></div>
            <!-- ko if: order.newReservationOrder -->
            <div class=""sales-tool-popup"">
                <a class=""sales-tool-button sales-tool-pull-right"" data-bind=""click: order.closeNewReservationOrder""><i class=""sales-tool-close""></i></a>
                <h2 data-bind=""text: getCulture('NewReservationOrderHeader')""></h2>
                <p><br /><span data-bind=""text: getCulture('NewReservationOrder')""></span></p>
                <p><br /><a class=""sales-tool-button"" data-bind=""text: getCulture('ShowReservationOrders') + ' >', click: order.getReservationList""></a></p>
            </div>
            <!-- /ko -->
            <nav class=""sales-tool-navbar sales-tool-divider"">
                <ul class=""sales-tool-navbar-nav sales-tool-pull-left"">
                    <li class=""sales-tool-navbar-header sales-tool-logo""></li>
                    <li>
                        <div class=""sales-tool-form-group"">
                            <input type=""text"" data-bind=""textInput: customer.searchText, event: { keypress: customer.searchEnter }, attr: { 'placeholder': getCulture('SearchCustomer') + '...' }"" />
                            <a class=""sales-tool-button"" data-bind=""visible: customer.searchText, click: customer.clear""><i class=""sales-tool-close""></i></a>
                            <a class=""sales-tool-search-button"" data-bind=""click: customer.search""></a>
                            <div class=""sales-tool-search-result"" data-bind=""visible: customer.showList"">
                                <table data-bind=""foreach: customer.list"">
                                    <tr data-bind=""click: $root.customer.set"">
                                        <td data-bind=""text: Name""></td>
                                        <td data-bind=""text: CompanyName""></td>
                                        <td data-bind=""text: Email""></td>
                                    </tr>
                                </table>
                            </div>                  
                        </div>
                    </li>
                </ul>
                <ul class=""sales-tool-navbar-nav sales-tool-pull-right"">
                    <li>
                        <div class=""sales-tool-button-group"" data-bind=""click: toggleAccount"">
                            <div>
                                <div class=""sales-tool-account-icon"" data-bind=""text: salesTool != null ? salesTool.AccountName().substr(0, 1).toUpperCase() : ''""></div>
                            </div>
                            <div>
                                <strong data-bind=""text: salesTool.AccountName""></strong>
                                <!-- ko if: salesTool.StoreName -->
                                <br /><small data-bind=""text: salesTool.StoreName""></small>
                                <!-- /ko -->
                            </div>
                            <div class=""sales-tool-dropdown-toggle"">
                                <i class=""sales-tool-caret""></i>
                            </div>
                            <ul class=""sales-tool-dropdown-menu"" data-bind=""visible: showAccount"">
                                <li class=""sales-tool-dropdown-info"">
                                    <strong data-bind=""text: getCulture('Store')""></strong><br />
                                    <div class=""sales-tool-button-group sales-tool-justified"">
                                        <a class=""sales-tool-button"" data-bind=""text: salesTool.StoreName() != null ? salesTool.StoreName() : getCulture('SelectStore'), click: selectStore""></a>
                                        <div class=""sales-tool-button sales-tool-dropdown-toggle"" data-bind=""click: selectStore"">
                                            <i class=""sales-tool-caret""></i>
                                        </div>
                                        <div class=""sales-tool-dropdown-menu sales-tool-scroll"" data-bind=""visible: stores().length > 0"">
                                            <ul data-bind=""foreach: stores"">
                                                <li><a data-bind=""text: Name, click: $root.setStore""></a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </li>
                                <li class=""sales-tool-dropdown-info""><small>Enferno Sales Tool " + Config.Version + @"</small></li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </nav>";
            if (config.UseOrders) {
                html += @"
            <nav class=""sales-tool-navbar sales-tool-divider-clear"">
                <ul class=""sales-tool-navbar-nav sales-tool-pull-left"">
                    <li class=""sales-tool-navbar-header""><h2 data-bind=""text: getCulture('Order')""></h2></li>
                    <li>
                        <div class=""sales-tool-form-group"">
                            <input type=""text"" data-bind=""textInput: order.searchText, event: { keypress: order.searchEnter }, attr: { 'placeholder': getCulture('SearchOrder') + '...' }"" />
                            <a class=""sales-tool-search-button"" data-bind=""click: order.search""></a>
                        </div>
                    </li>
                </ul>
                <ul class=""sales-tool-navbar-nav sales-tool-pull-right"">
                    <li class=""sales-tool-button-group"" data-bind=""visible: hasCustomer"">
                        <div class=""sales-tool-button-group"">
                            <a class=""sales-tool-button"" data-bind=""click: order.getList""><i class=""sales-tool-order""></i><span data-bind=""text: getCulture('CustomerOrders')""></span></a>
                        </div>
                    </li>";
                if(config.UseStorePickup)
                {
                    html += @"
                    <li class=""sales-tool-button-group"" data-bind=""visible: hasStore"">
                        <div class=""sales-tool-button-group"">
                            <a class=""sales-tool-button"" data-bind=""click: order.getPickupList"">
                                <!-- ko if: order.pickUpCount -->
                                <span class=""sales-tool-badge"" data-bind=""text: order.pickUpCount""></span>
                                <!-- /ko -->
                                <i class=""sales-tool-order""></i>
                                <span data-bind=""text: getCulture('PickupOrders')""></span>
                            </a>
                        </div>
                    </li>";
                }
                if (config.UseStoreReservation)
                {
                    html += @"
                    <li class=""sales-tool-button-group"" data-bind=""visible: hasStore"">
                        <div class=""sales-tool-button-group"">
                            <a class=""sales-tool-button"" data-bind=""click: order.getReservationList"">
                                <!-- ko if: order.reservationCount -->
                                <span class=""sales-tool-badge"" data-bind=""text: order.reservationCount""></span>
                                <!-- /ko -->
                                <i class=""sales-tool-order""></i>
                                <span data-bind=""text: getCulture('ReservationOrders')""></span>
                            </a>
                        </div>
                    </li>";
                }
                html += @"
                </ul>
            </nav>
            <div class=""sales-tool-list"" data-bind=""visible: order.showList"">
                <div class=""sales-tool-pull-right sales-tool-margin"">
                    <div class=""sales-tool-button-group"">
                        <div class=""sales-tool-button"" data-bind=""text: order.filterLabel, click: order.toggleFilter""></div>
                        <div class=""sales-tool-button sales-tool-dropdown-toggle"" data-bind=""click: order.toggleFilter"">
                            <i class=""sales-tool-caret""></i>
                        </div>
                        <ul class=""sales-tool-dropdown-menu"" data-bind=""visible: order.showFilter"">
                            <li><a data-bind=""text: getCulture('ShowAll'), click: $root.order.setFilter('ShowAll')""></a></li>
                            <li><a data-bind=""text: getCulture('Incoming'), click: $root.order.setFilter('Confirmed')""></a></li>";
                if (config.UseStorePickup)
                {
                    html += @"
                            <li><a data-bind=""text: getCulture('ReadyForPickup'), click: $root.order.setFilter('ReadyForPickup')""></a></li>";
                }
                if (config.UseStoreReservation)
                {
                    html += @"
                            <li><a data-bind=""text: getCulture('ReadyForReservation'), click: $root.order.setFilter('ReadyForReservation')""></a></li>
                            <li><a data-bind=""text: getCulture('Reserved'), click: $root.order.setFilter('Reserved')""></a></li>";
                }
                if (config.UseStorePickup || config.UseStoreReservation)
                {
                    html += @"
                            <li><a data-bind=""text: getCulture('PickedUp'), click: $root.order.setFilter('PickedUp')""></a></li>";
                }
                html += @"
                            <li><a data-bind=""text: getCulture('Cancelled'), click: $root.order.setFilter('Cancelled')""></a></li>
                        </ul>
                    </div>
                    <a class=""sales-tool-button"" data-bind=""click: closeWindows""><i class=""sales-tool-close""></i></a>
                </div>
                <h1 data-bind=""text: order.header""></h1>
                <table class=""sales-tool-divider"">
                    <tr>
                        <th></th>
                        <th data-bind=""text: getCulture('OrderNo')""></th>
                        <th data-bind=""text: getCulture('Date')""></th>
                        <th data-bind=""text: getCulture('Company')""></th>
                        <th data-bind=""text: getCulture('Customer')""></th>
                        <th data-bind=""text: getCulture('Email')""></th>
                        <th data-bind=""text: getCulture('Shipping')""></th>
                        <th data-bind=""text: getCulture('Total')"" class=""sales-tool-text-right""></th>
                        <th data-bind=""text: getCulture('Status')""></th>
                    </tr>
                    <!-- ko foreach: order.list -->
                    <tr data-bind=""click: $root.order.set"">
                        <td>";
                if (config.NotifyOrder)
                    html += @"
                        <!-- ko if: $root.order.getNextStatus(Status) == 'PickedUp' -->
                        <input type='checkbox' data-bind=""checked: Checked, stopBubble: true"" />
                        <!-- /ko -->";
                html += @"
                        </td>
                        <td data-bind=""text: OrderNumber""></td>
                        <td data-bind=""text: $root.formatDate(OrderDate)"" class=""sales-tool-nowrap""></td>
                        <td data-bind=""text: CompanyName""></td>
                        <td data-bind=""text: CustomerName""></td>
                        <td data-bind=""text: Email""></td>
                        <td data-bind=""text: DeliveryMethodName""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: $root.format(Total)""></td>
                        <td data-bind=""text: $root.getCulture(Status)""></td>
                    </tr>
                    <!-- /ko -->
                </table>";
                if (config.NotifyOrder)
                    html += @"
                <table>
                    <tr>
                        <th><a class=""sales-tool-button"" data-bind=""text: getCulture('NotifyCheckedOrders'), click: order.notifyCheckedOrders""></a></th>
                    </tr>
                </table>";
                html += @"
            </div>
            <div class=""sales-tool-divider"" data-bind=""visible: order.order"">
                <div class=""sales-tool-pull-right sales-tool-margin"">";
                if (config.NotifyOrder)
                    html += @"
                        <!-- ko if: order.order() != null && order.getNextStatus(order.order().Status) == 'PickedUp' -->
                        <a class=""sales-tool-button"" data-bind=""click: $root.order.notify""><i class=""sales-tool-email""></i></a>
                        <!-- /ko -->";
                html += @"
                    <a class=""sales-tool-button"" data-bind=""click: order.print""><i class=""sales-tool-print""></i></a>
                    <a class=""sales-tool-button"" data-bind=""click: order.close""><i class=""sales-tool-close""></i></a>
                </div>
                <h1 data-bind=""text: order.itemHeader""></h1>
                <div class=""sales-tool-col-one-third"">
                    <label data-bind=""text: getCulture('OrderDate') + ':'""></label>
                    <span data-bind=""text: order.order() ? $root.formatDate(order.order().OrderDate) : ''""></span><br />
                    <label data-bind=""text: getCulture('Status') + ':'""></label>
                    <span data-bind=""text: order.order() ? getCulture(order.order().Status) : ''""></span><br />
                    <label data-bind=""text: getCulture('Shipping') + ':'""></label>
                    <span data-bind=""text: order.order() ? order.order().DeliveryMethodName : ''""></span><br />
                    <!-- ko if: order.order() && order.order().PickupStore -->
                    <label data-bind=""text: getCulture('Store') + ':'""></label>
                    <span data-bind=""text: order.order() ? order.order().PickupStore.Name : ''""></span><br />
                    <!-- /ko -->
                    <label data-bind=""text: getCulture('Payment') + ':'""></label>
                    <span data-bind=""text: order.order() ? order.order().PaymentMethodName : ''""></span><br />
                </div>
                <div class=""sales-tool-col-one-third"">
                    <label data-bind=""text: getCulture('Buyer')""></label><br />
                    <span data-bind=""html: order.order() && order.order().Buyer.Name ? order.order().Buyer.Name + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().Buyer.Email ? order.order().Buyer.Email + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().Buyer.Phone ? order.order().Buyer.Phone + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().BuyerAddress.Address1 ? order.order().BuyerAddress.Address1 + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().BuyerAddress.Address2 ? order.order().BuyerAddress.Address2 + '<br />' : ''""></span>
                    <span data-bind=""text: order.order() ? order.order().BuyerAddress.Zip : ''""></span>
                    <span data-bind=""text: order.order() ? order.order().BuyerAddress.City : ''""></span><br />
                    <span data-bind=""text: order.order() ? order.order().BuyerAddress.Country : ''""></span>
                </div>
                <div class=""sales-tool-col-one-third"">
                    <label data-bind=""text: getCulture('ShipTo')""></label><br />
                    <span data-bind=""html: order.order() && order.order().ShipTo.Name ? order.order().ShipTo.Name + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().ShipToAddress.Address1 ? order.order().ShipToAddress.Address1 + '<br />' : ''""></span>
                    <span data-bind=""html: order.order() && order.order().ShipToAddress.Address2 ? order.order().ShipToAddress.Address2 + '<br />' : ''""></span>
                    <span data-bind=""text: order.order() ? order.order().ShipToAddress.Zip : ''""></span>
                    <span data-bind=""text: order.order() ? order.order().ShipToAddress.City : ''""></span><br />
                    <span data-bind=""text: order.order() ? order.order().ShipToAddress.Country : ''""></span>
                </div>
                <!-- ko foreach: order.order() ? order.order().DeliveryNotes : [] -->
                <div class=""sales-tool-divider""></div>
                <div class=""sales-tool-col-one-third sales-tool-top-margin"">
                    <label data-bind=""text: $root.getCulture('DeliveryNote') + ':'""></label>
                    <span data-bind=""text: Number""></span>
                </div>
                <div class=""sales-tool-col-one-third sales-tool-top-margin"">
                    <label data-bind=""text: $root.getCulture('ParcelCode') + ':'""></label>
                    <span data-bind=""text: ParcelCode""></span>
                </div>
                <div class=""sales-tool-col-one-third sales-tool-text-right"">
                    <label data-bind=""text: $root.getCulture('Status') + ':'""></label>
                    <span data-bind=""text: $root.getCulture(Status)""></span>";
                if (config.NotifyOrder)
                    html += @"
                    <!-- ko if: $root.order.getChangeToLabel(Status) -->
                    <div class=""sales-tool-button"" data-bind=""text: $root.order.getChangeToLabel(Status), click: $root.order.changeDeliveryNoteStatus""></div>
                    <!-- /ko -->";
                html += @"                
                    <!-- ko if: $root.order.order() && $root.order.order().Status != 'Cancelled' && $root.order.order().Status != 'PickedUp' -->
                    <div class=""sales-tool-button"" data-bind=""text: $root.getCulture('Cancel'), click: $root.order.cancel""></div>
                    <!-- /ko -->
                </div>
                <table class=""sales-tool-divider"">
                    <tr>
                        <th data-bind=""text: $root.getCulture('PartNo')""></th>
                        <th data-bind=""text: $root.getCulture('Name')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Price')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Discount')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Quantity')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Total')""></th>
                    </tr>
                    <!-- ko foreach: Rows -->
                    <tr>
                        <td data-bind=""text: PartNo""></td>
                        <td data-bind=""text: Name""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: $root.format(Price)""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: Discount + '%'""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: Quantity""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: $root.format(Total)""></td>
                    </tr>
                    <!-- /ko -->
                </table>
                <!-- /ko -->
                <!-- ko if: order.order() && order.order().NoDeliveryNoteRows.length > 0 -->
                <div class=""sales-tool-col-one-whole sales-tool-top-margin"" data-bind=""text: $root.getCulture('DeliveryNoteNotCreated') + ':'""></div>
                <table class=""sales-tool-divider"">
                    <tr>
                        <th data bind=""text: $root.getCulture('PartNo')""></th>
                        <th data-bind=""text: $root.getCulture('Name')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Price')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Discount')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Quantity')""></th>
                        <th class=""sales-tool-text-right"" data-bind=""text: $root.getCulture('Total')""></th>
                    </tr>
                    <!-- ko foreach: order.order().NoDeliveryNoteRows -->
                    <tr>
                        <td data-bind=""text: PartNo""></td>
                        <td data-bind=""text: Name""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: $root.format(Price)""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: Discount + '%'""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: Quantity""></td>
                        <td class=""sales-tool-text-right sales-tool-nowrap"" data-bind=""text: $root.format(Total)""></td>
                    </tr>
                    <!-- /ko -->
                </table>
                <!-- /ko -->
                <table>
                    <tr>
                        <th><label data-bind=""text: $root.getCulture('Vat') + ':'""></label> <span data-bind=""text: $root.format(order.order() ? order.order().Vat : 0)""></span></th>
                        <th class=""sales-tool-text-right""><label data-bind=""text: $root.getCulture('Total').toUpperCase() + ':'""></label> <span data-bind=""text: $root.format(order.order() ? order.order().Total : 0)""></span></th>
                    </tr>
                </table>
            </div>";
            }
            html += @"
        </div>
        <iframe id=""sales-tool-print""></iframe>
        <div id=""sales-tool-pusher""></div>";
            return html;
        }
    }
}