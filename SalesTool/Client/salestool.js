var Enferno;
(function (Enferno) {
    var Public;
    (function (Public) {
        var Web;
        (function (Web) {
            var SalesTool;
            (function (SalesTool) {
                var Culture = function () {
                    var self = this;

                    self.getCulture = function (key, cultureCode) {
                        if (self.cultureStrings[cultureCode])
                            return self.cultureStrings[cultureCode][key];
                        return self.cultureStrings['en'][key];
                    };

                    self.cultureStrings = {
                        'en': {
                            'Acknowledged': 'Notified',
                            'Allocation': 'Allocation',
                            'BackOrder': 'Back order',
                            'Buyer': 'Buyer',
                            'Cancel': 'Cancel order',
                            'Cancelled': 'Cancelled',
                            'ChangeTo': 'Change to',
                            'Company': 'Company',
                            'Confirmed': 'Placed order',
                            'CreditControl': 'Credit control',
                            'Customer': 'Customer',
                            'CustomerOrders': 'Customer orders',
                            'Date': 'Date',
                            'DecimalSign': '.',
                            'Delivered': 'Delivered',
                            'DeliveryNote': 'Delivery note',
                            'DeliveryNoteNotCreated': 'Delivery note not created',
                            'Discount': 'Discount',
                            'Email': 'Email',
                            'ErpConfirmed': 'Confirmed',
                            'Filter': 'Filter',
                            'Invoiced': 'Invoiced',
                            'Name': 'Name',
                            'NotifyCheckedOrders': 'Remind checked orders',
                            'NotifiedOrder': 'Reminders has been sent to customer.',
                            'NotifiedOrders': 'Reminders has been sent to checked orders.',
                            'NotPickedUp': 'Not picked up',
                            'Order': 'Order',
                            'OrderDate': 'Order date',
                            'OrderNo': 'Orderno.',
                            'Orders': 'Orders',
                            'OrdersFor': 'Orders for',
                            'ParcelCode': 'Parcel code',
                            'PartlyDelivered': 'Partly delivered',
                            'PartNo': 'Partno.',
                            'Payment': 'Payment',
                            'PickedUp': 'Picked up',
                            'PickupOrders': 'Pick up',
                            'CellPhone': 'Cell phone',
                            'Price': 'Price',
                            'Quantity': 'Quantity',
                            'ReadyForPickup': 'Ready for pickup',
                            'ReadyForReservation': 'Ready for reservation',
                            'ReservationOrders': 'Reservation',
                            'Reserved': 'Reserved',
                            'SearchCustomer': 'Search customer',
                            'SearchOrder': 'Search order',
                            'SelectStore': 'Select store...',
                            'Shipping': 'Shipping',
                            'ShipTo': 'Ship to',
                            'ShowAll': 'Show all',
                            'Status': 'Status',
                            'Store': 'Store',
                            'ThousandSeparator': ',',
                            'Total': 'Total',
                            'UndeliveredOrders': 'Undelivered orders for store',
                            'UndeliveredOrdersFor': 'Undelivered orders for',
                            'Unknown': 'Unknown',
                            'Vat': 'VAT',
                        },
                        'sv': {
                            'Acknowledged': 'Meddelad',
                            'Allocation': 'Plockas',
                            'BackOrder': 'Restorder',
                            'Buyer': 'Köpare',
                            'Cancel': 'Avbeställ order',
                            'Cancelled': 'Avbeställd',
                            'ChangeTo': 'Ändra till',
                            'Company': 'Företag',
                            'Confirmed': 'Beställd',
                            'CreditControl': 'Kreditkontroll',
                            'Customer': 'Person',
                            'CustomerOrders': 'Kundorder',
                            'Date': 'Datum',
                            'DecimalSign': ',',
                            'Delivered': 'Levererad',
                            'DeliveryNote': 'Följesedel',
                            'DeliveryNoteNotCreated': 'Följesedel ej skapad',
                            'Discount': 'Rabatt',
                            'Email': 'E-post',
                            'ErpConfirmed': 'Godkänd',
                            'Filter': 'Filter',
                            'Invoiced': 'Fakturerad',
                            'Name': 'Namn',
                            'NotifyCheckedOrders': 'Påminn markerade order',
                            'NotifiedOrder': 'Påminnelser har skickats till kunden.',
                            'NotifiedOrders': 'Påminnelser har skickats till markerade order.',
                            'NotPickedUp': 'Återlämnad',
                            'Order': 'Order',
                            'OrderDate': 'Orderdatum',
                            'OrderNo': 'Ordernr.',
                            'Orders': 'Order',
                            'OrdersFor': 'Order för',
                            'ParcelCode': 'Kollinummer',
                            'PartlyDelivered': 'Dellevererad',
                            'PartNo': 'Art.nr',
                            'Payment': 'Betalsätt',
                            'PickedUp': 'Hämtad',
                            'PickupOrders': 'Hämtas',
                            'CellPhone': 'Mobiltelefon',
                            'Price': 'Pris',
                            'Quantity': 'Antal',
                            'ReadyForPickup': 'Inlevererad',
                            'ReadyForReservation': 'Ska reserveras',
                            'ReservationOrders': 'Reservation',
                            'Reserved': 'Reserverad',
                            'SearchCustomer': 'Sök kund',
                            'SearchOrder': 'Sök order',
                            'SelectStore': 'Välj butik...',
                            'Shipping': 'Leveranssätt',
                            'ShipTo': 'Leverans',
                            'ShowAll': 'Visa alla',
                            'Status': 'Status',
                            'Store': 'Butik',
                            'ThousandSeparator': ' ',
                            'Total': 'Totalt',
                            'UndeliveredOrders': 'Olevererade order för butik',
                            'UndeliveredOrdersFor': 'Olevererade order för',
                            'Unknown': 'Okänd',
                            'Vat': 'Moms',
                        },
                    };
                }
                SalesTool.Culture = Culture;

                var Customer = function (parent) {
                    var self = this;
                    self.parent = parent;
                    self.list = ko.observableArray();
                    self.searchText = ko.observable();
                    self.searchTextDelay = ko.pureComputed(self.searchText).extend({ rateLimit: { method: 'notifyWhenChangesStop', timeout: 400 } });
                    self.showList = ko.pureComputed(function () { return self.list().length > 0; }, self);

                    self.searchTextDelay.subscribe(function (val) {
                        self.search();
                    }, this);

                    self.searchEnter = function (data, event) {
                        if (event.keyCode !== 13 || self.list().length === 0)
                            return true;
                        data = { Id: self.list()[0].Id };
                        self.set(data);
                    }

                    self.search = function () {
                        var val = self.searchText();
                        if (val.length < 3) {
                            self.list([]);
                            return;
                        }
                        if (val === self.parent.salesTool.CustomerName())
                            return;
                        self.parent.isLoading(true);
                        $.get('/api/salestool/customer/search/' + val)
                        .done(function (customerItems) {
                            self.list(customerItems);
                            self.parent.setAutoClose(event);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.set = function (data) {
                        self.parent.closeWindows();
                        self.parent.isLoading(true);
                        $.ajax({
                            url: '/api/salestool/customer/set/' + data.Id,
                            type: 'PUT',
                        })
                        .done(function (salesTool) {
                            self.parent.dataBind(salesTool);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    };

                    self.clear = function () {
                        self.searchText(null);
                        self.parent.closeWindows();
                        if (self.parent.hasCustomer()) {
                            self.parent.isLoading(true);
                            $.ajax({
                                url: '/api/salestool/customer/clear',
                                type: 'PUT',
                            })
                            .done(function (salesTool) {
                                self.parent.dataBind(salesTool);
                            })
                            .fail(function (error) {
                                self.parent.showError(error);
                            })
                            .complete(function () {
                                self.parent.isLoading(false);
                            });
                        }
                    };
                }
                SalesTool.Customer = Customer;

                var Order = function (parent) {
                    var self = this;

                    var applyFilter = function (item) {
                        if (self.filter() === 'ShowAll')
                            return true;
                        return self.compareStatus(item.Status, self.filter());
                    };

                    self.parent = parent;
                    self.showMenu = ko.observable(false);
                    self.searchText = ko.observable();
                    self.list = ko.pureComputed(function () {
                        return self.showStoreList() ? self.unfilteredStoreList().filter(applyFilter) : self.unfilteredList().filter(applyFilter);
                    }, self);
                    self.unfilteredList = ko.observableArray([]);
                    self.unfilteredStoreList = ko.observableArray([]);
                    self.loadedStore = 0;
                    self.pickUpCount = ko.pureComputed(function () {
                        return self.unfilteredStoreList().filter(function (item) { return self.compareStatus(item.Status, 'Confirmed') }).length;
                    }, self);
                    self.reservationCount = ko.pureComputed(function () {
                        return self.unfilteredStoreList().filter(function (item) { return self.compareStatus(item.Status, 'ReadyForReservation') }).length;
                    }, self);
                    self.showStoreList = ko.observable(false);
                    self.filter = ko.observable('Confirmed');
                    self.showFilter = ko.observable(false);
                    self.filterLabel = ko.pureComputed(function () { return self.parent.getCulture('Filter') + (self.filter() ? ': ' + self.parent.getCulture(self.filter()) : ''); }, self);
                    self.showList = ko.pureComputed(function () { return (self.showStoreList() ? self.unfilteredStoreList().length : self.unfilteredList().length) > 0 && self.order() == null; }, self);
                    self.order = ko.observable();
                    self.header = ko.observable();
                    self.itemHeader = ko.observable();

                    self.toggleMenu = function (data, event) {
                        if (!self.showMenu())
                            self.parent.setAutoClose(event);
                        self.showMenu(!self.showMenu());
                    }

                    self.getList = function (data, event) {
                        self.header(self.parent.getCulture('OrdersFor') + ' ' + self.parent.salesTool.CustomerName());
                        self.filter('ShowAll');
                        self.parent.closeWindows();
                        self.parent.isLoading(true);
                        self.parent.preventBubble(data, event);
                        self.showStoreList(false);
                        $.get('/api/salestool/order/list')
                        .done(function (list) {
                            self.unfilteredList(list);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    };

                    self.getPickupList = function (data, event) {
                        self.header(self.parent.getCulture('OrdersFor') + ' ' + self.parent.salesTool.StoreName());
                        self.filter('Confirmed');
                        self.parent.closeWindows();
                        self.parent.preventBubble(data, event);
                        self.loadStoreList();
                    };

                    self.getReservationList = function (data, event) {
                        self.header(self.parent.getCulture('OrdersFor') + ' ' + self.parent.salesTool.StoreName());
                        self.filter('ReadyForReservation');
                        self.parent.closeWindows();
                        self.parent.preventBubble(data, event);
                        self.loadStoreList();
                    };

                    self.loadStoreList = function () {
                        self.showStoreList(true);
                        if (self.loadedStore == self.parent.salesTool.StoreId())
                            return;
                        self.parent.isLoading(true);
                    }
                        
                    self.loadStoreListImpl = function () {
                        $.get('/api/salestool/order/liststore')
                        .done(function (list) {
                            self.unfilteredStoreList(list);
                            self.loadedStore = self.parent.salesTool.StoreId();
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.searchEnter = function (data, event) {
                        if (event.keyCode != 13 || self.searchText().length < 3)
                            return true;
                        self.search();
                    }

                    self.search = function () {
                        self.parent.isLoading(true);
                        $.get('/api/salestool/order/search/' + self.searchText())
                        .done(function (order) {
                            self.setOrder(order);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.set = function (data) {
                        self.itemHeader(self.parent.getCulture('Order') + ' ' + data.OrderNumber);
                        self.parent.preventBubble(data, event);
                        self.parent.isLoading(true);
                        $.get('/api/salestool/order/get/' + data.OrderNumber)
                        .done(function (order) {
                            self.setOrder(order);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    };

                    self.setOrder = function (order) {
                        self.order(order);
                    }

                    self.close = function() {
                        self.order(null);
                    }

                    self.toggleFilter = function (data, event) {
                        self.showFilter(!self.showFilter());
                    }

                    self.setFilter = function (filter) {
                        self.filter(filter);
                        self.showFilter(false);
                    }

                    self.getChangeToLabel = function (status) {
                        var nextStatus = self.getNextStatus(status);
                        if (nextStatus != null)
                            return parent.getCulture('ChangeTo') + ' ' + parent.getCulture(nextStatus);
                        return null;
                    }

                    self.compareStatus = function (a, b) {
                        if (a == b)
                            return true;
                        switch (a) {
                            case 'ErpConfirmed':
                            case 'Invoiced':
                            case 'PartlyDelivered':
                                return b == 'Confirmed';
                            case 'Acknowledged':
                            case 'NotPickedUp':
                                return b == 'ReadyForPickup';
                        }
                        return false;
                    }

                    self.getNextStatus = function (status) {
                        /* Unknown = 0,
                        Allocation = 1,
                        Confirmed = 2,
                        BackOrder = 3,
                        Delivered = 4,
                        Invoiced = 5,
                        Cancelled = 6,
                        CreditControl = 7,
                        PartlyDelivered = 8,
                        Acknowledged = 9, // ReadyForPickup and notified
                        ErpConfirmed = 10, // Purchase order is saved in Client ERP
                        ReadyForPickup = 11, // Packages is ready for pick up at store
                        PickedUp = 12, // Packages is picked up at store
                        NotPickedUp = 13 // Packages was returned and not picked up at store
                        OnHold = 14 // Purchase order is sent to supplier but will not be processed before it will be released
                        WaitingForCancel = 15 // Purchase order cancellation is sent, waiting for confirmation of receipt
                        WaitingForOnHoldRemoved = 16 // Remove OnHold flag request is sent to supplier, waiting for confirmation of receipt
                        CancelError = 17 // Cancellation attempt failed
                        AcknowledgementError = 18 // Purchase order creation attempt failed
                        ReadyForReservation = 19 // Items is ready to be reserved in Store
                        Reserved = 20 // Items is reserved in Store */
                        switch(status) {
                            case 'Confirmed':
                            case 'ErpConfirmed':
                            case 'Invoiced':
                            case 'PartlyDelivered':
                                return 'ReadyForPickup'; break;
                            case 'ReadyForPickup':
                            case 'Acknowledged':
                            case 'NotPickedUp':
                                return 'PickedUp'; break;
                            case 'PickedUp':
                                return 'Delivered'; break;
                            case 'ReadyForReservation':
                                return 'Reserved'; break;
                            case 'Reserved':
                                return 'Delivered'; break;
                            case 'Delivered':
                            case 'Cancelled':
                                return 'ReadyForPickup'; break;
                        }
                        return null;
                    }

                    self.changeDeliveryNoteStatus = function (data, event) {
                        self.setStatus(data, self.getNextStatus(data.Status));
                    }

                    self.cancel = function (data, event) {
                        self.setStatus(data, "Cancelled");
                    }

                    self.setStatus = function (data, status) {
                        if (self.parent.isLoading())
                            return false;
                        self.parent.isLoading(true);
                        data.Status = status;
                        $.ajax({
                            url: '/api/salestool/order/updatedeliverynotestatus',
                            type: 'PUT',
                            data: JSON.stringify(data),
                            contentType: 'application/json',
                        })
                        .done(function (order) {
                            self.order(order);
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.notifyCheckedOrders = function () {
                        if (self.parent.isLoading())
                            return false;
                        self.parent.isLoading(true);
                        $.ajax({
                            url: '/api/salestool/order/notifycheckedorders',
                            type: 'PUT',
                            data: JSON.stringify(self.unfilteredList()),
                            contentType: 'application/json',
                        })
                        .done(function (list) {
                            self.unfilteredList(list);
                            self.parent.showMessage(self.parent.getCulture('NotifiedOrders'));
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.notify = function () {
                        if (self.parent.isLoading())
                            return false;
                        self.parent.isLoading(true);
                        $.ajax({
                            url: '/api/salestool/order/notifycheckedorder',
                            type: 'PUT',
                            data: JSON.stringify(self.order()),
                            contentType: 'application/json',
                        })
                        .done(function () {
                            self.parent.showMessage(self.parent.getCulture('NotifiedOrder'));
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    self.print = function () {
                        if (self.parent.isLoading())
                            return false;
                        self.parent.isLoading(true);
                        $.get('/api/salestool/order/print/' + self.order().OrderNumber)
                        .done(function (text) {
                            $('#sales-tool-print').contents().find('body').html(text);
                            $('#sales-tool-print').get(0).contentWindow.print();
                        })
                        .fail(function (error) {
                            self.parent.showError(error);
                        })
                        .complete(function () {
                            self.parent.isLoading(false);
                        });
                    }

                    // Initialize
                    self.loadStoreListImpl();
                    window.setInterval(function () { self.loadStoreListImpl(); }, 300000); // Every 5 minutes
                }
                SalesTool.Order = Order;

                var Main = function () {
                    var self = this;
                    self.salesTool = null;
                    self.isLoading = ko.observable(true);
                    self.isLoaded = ko.observable(false);
                    self.showAccount = ko.observable(false);
                    self.culture = new SalesTool.Culture();
                    self.customer = new SalesTool.Customer(self);
                    self.order = new SalesTool.Order(self);
                    self.stores = ko.observableArray();
                    self.hasCustomer = ko.pureComputed(function () { return self.salesTool != null ? self.salesTool.CustomerId() > 0 : false; });
                    self.hasStore = ko.pureComputed(function () { return self.salesTool != null ? self.salesTool.StoreId() > 0 : false; });
                    self.ticks = 0;

                    self.dataBind = function (salesTool) {
                        if (self.salesTool == null)
                            self.salesTool = ko.mapping.fromJS(salesTool);
                        else
                            ko.mapping.fromJS(salesTool, self.salesTool);
                        self.closeWindows();
                    };

                    self.toggleAccount = function (data, event) {
                        if (!self.showAccount())
                            self.setAutoClose(event);
                        self.showAccount(!self.showAccount());
                    }

                    self.selectStore = function (data, event) {
                        if (event.timeStamp - self.ticks > 1000) {
                            self.setAutoClose(event);
                            $.get('/api/salestool/liststores')
                            .done(function (stores) {
                                self.stores(stores);
                            })
                            .fail(function (error) {
                                self.showError(error);
                            });
                        }
                    };

                    self.setStore = function (data, event) {
                        self.ticks = event.timeStamp;
                        self.closeWindows();
                        self.isLoading(true);
                        $.post('/api/salestool/setstore', { '': data.Id })
                        .done(function (salesTool) {
                            self.dataBind(salesTool);
                        })
                        .fail(function (error) {
                            self.showError(error);
                        });
                    };

                    self.format = function (number) {
                        var split = String(number).split('.');
                        var value = split[0];
                        var decimal = split.length > 1 ? split[1] : '00';
                        var pos = value.length - 3;
                        while (pos > 0) {
                            value = [value.slice(0, pos), self.getCulture('ThousandSeparator'), value.slice(pos)].join('');
                            pos -= 3;
                        }
                        if (decimal.length == 1)
                            decimal = decimal + '0';
                        else
                            decimal = decimal.substr(0, 2);
                        return value + self.getCulture('DecimalSign') + decimal;
                    };

                    self.getCulture = function (key) {
                        return self.culture.getCulture(key, self.salesTool ? self.salesTool.CultureCode() : 'en');
                    };

                    self.showMessage = function (message) {
                        self.isLoading(false);
                        alert(message);
                    };

                    self.showError = function (error) {
                        self.isLoading(false);
                        alert(error.responseText);
                    };

                    self.closeWindows = function () {
                        $('html').off('click');
                        self.customer.list([]);
                        self.showAccount(false);
                        self.stores([]);
                        self.order.showMenu(false);
                        self.order.unfilteredList([]);
                        self.order.showStoreList(false);
                        self.order.order(null);
                        self.order.searchText(null);
                        self.order.showFilter(false);
                        self.customer.searchText(self.salesTool.CustomerName());
                        self.isLoading(false);
                    };

                    self.preventBubble = function (data, event) {
                        event.cancelBubble = true;
                        if (event.stopPropagation) {
                            event.stopPropagation();
                        }
                    }

                    self.setAutoClose = function (event) {
                        $('html').one('click', function () {
                            enferno_salestool.closeWindows();
                        });
                        event.stopPropagation();
                    };

                    // Initialize component
                    $.get('/api/salestool/get')
                    .done(function (salesTool) {
                        if (salesTool != null) {
                            self.dataBind(salesTool);
                            ko.applyBindings(self, $('#sales-tool')[0]);
                            self.isLoaded(true);
                        } else {
                            $('#sales-tool').hide();
                        }
                    })
                    .fail(function (error) {
                        self.showError(error);
                    });
                };
                SalesTool.Main = Main;
            })(SalesTool = Web.SalesTool || (Web.SalesTool = {}));
        })(Web = Public.Web || (Public.Web = {}));
    })(Public = Enferno.Public || (Enferno.Public = {}));
})(Enferno || (Enferno = {}));

ko.bindingHandlers.stopBubble = {
    init: function (element) {
        ko.utils.registerEventHandler(element, 'click', function (event) {
            event.cancelBubble = true;
            if (event.stopPropagation) {
                event.stopPropagation();
            }
        });
    }
};

var enferno_salestool;
$(document).ready(function () {
    enferno_salestool = new Enferno.Public.Web.SalesTool.Main();
});
