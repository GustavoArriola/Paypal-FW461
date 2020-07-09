using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paypal_FW461
{
    public partial class Default : Shared.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Start the Paypal flow.
        /// </summary>
        protected void btnPayNow_Click(object sender, EventArgs e)
        {
            string urlToBuyerAutorize = String.Empty;
            PaypalFlow.Orders paypalFlowOrders = new PaypalFlow.Orders();
            //All is async...
            Task.Run(async () =>
            {
                //This will create an order and print order id for the created order.
                //The id we will need in the next stage, when paypal redirects the buyer to our site again.
                Order order = await paypalFlowOrders.CreateOrder();

                //I save the order identifier to use again after the buyer authorizes the payment.
                //Once the payment is authorized by the buyer, I must inform Paypal.
                this.SaveInDatabase("PaypalOrderId", order.Id); //You can relate the Paypal identifier to your internal identifier of your system.

                //We need to redirect the buyer to Paypal to pay. So we need the approval URL ...
                foreach (var item in order.Links)
                {
                    if (item.Rel.ToLower() == "approve")
                    {
                        urlToBuyerAutorize = item.Href;
                        break;
                    }
                }
            }).Wait();
            if (!String.IsNullOrWhiteSpace(urlToBuyerAutorize))
            {
                //We send the buyer to the paypal page so that he can apply the 
                //payment and select his preferred payment method.
                Response.Redirect(urlToBuyerAutorize);
            }
        }
    }
}