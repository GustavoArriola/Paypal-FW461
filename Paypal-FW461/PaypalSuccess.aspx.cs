﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Paypal_FW461
{
    public partial class PaypalSuccess : Shared.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                String orderId = ConfigurationManager.AppSettings["PaypalOrderId"];
                //The buyer's ID, you can use it to associate the order's id with the buyer's id.
                String payerID = Request.QueryString["PayerID"];
                //this.SaveInDatabase("PaypalPayerId", payerID);                
                Task.Run(async () =>
                {
                    PaypalFlow.Orders orders = new PaypalFlow.Orders();
                    //Use the Capture Order API to execute the payment...
                    await orders.CaptureOrder(orderId);
                }).Wait();
            }            
        }
    }
}