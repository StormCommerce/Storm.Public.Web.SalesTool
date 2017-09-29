using Enferno.Web.StormUtils;
using System;

namespace Enferno.Public.Web.SalesTool
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(StormContext.AccountId.GetValueOrDefault() == 0)
            {
                StormContext.AccountId = 1449278;
                StormContext.DivisionId = 581;
                StormContext.CultureCode = "sv-SE";
                StormContext.CurrencyId = 2;
                StormContext.ShowPricesIncVat = true;
            }
        }
    }
}