using System;
using System.Configuration;

namespace Enferno.Public.Web.SalesTool
{
    public class SalesToolSection : ConfigurationSection
    {
        [ConfigurationProperty("useStorePickup", DefaultValue = "false", IsRequired = false)]
        public bool UseStorePickup
        {
            get
            {
                return (bool)this["useStorePickup"];
            }
            set
            {
                this["useStorePickup"] = value;
            }
        }
        [ConfigurationProperty("useStoreReservation", DefaultValue = "false", IsRequired = false)]
        public bool UseStoreReservation
        {
            get
            {
                return (bool)this["useStoreReservation"];
            }
            set
            {
                this["useStoreReservation"] = value;
            }
        }
        [ConfigurationProperty("useOrders", DefaultValue = "true", IsRequired = false)]
        public bool UseOrders
        {
            get
            {
                return (bool)this["useOrders"];
            }
            set
            {
                this["useOrders"] = value;
            }
        }
        [ConfigurationProperty("notifyOrder", DefaultValue = "false", IsRequired = false)]
        public bool NotifyOrder
        {
            get
            {
                return (bool)this["notifyOrder"];
            }
            set
            {
                this["notifyOrder"] = value;
            }
        }

        [ConfigurationProperty("deliveryNotePrintXsltPath", DefaultValue = null, IsRequired = false)]
        public string DeliveryNotePrintXsltPath
        {
            get
            {
                return (string)this["deliveryNotePrintXsltPath"];
            }
            set
            {
                this["deliveryNotePrintXsltPath"] = value;
            }
        }
    }
}