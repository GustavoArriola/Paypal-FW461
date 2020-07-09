using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Paypal_FW461.PaypalFlow
{ 
    public class Orders
    {
        #region Create Order
        /// <summary>
        /// Create a Paypal order
        /// </summary>
        /// <remarks>
        /// All calls to Execute are async.
        /// </remarks>
        /// <returns>Return a PaypalCheckoutSdk.Orders.Order</returns>
        public async Task<Order> CreateOrder()
        {
            //Check https://developer.paypal.com/docs/api/orders/v2/

            var request = new OrdersCreateRequest();
            //return=minimal: The server returns a minimal response to optimize communication between the API caller and the server. 
            //return=representation:  The server returns a complete resource representation.
            request.Headers.Add("prefer", "return=representation"); 
            request.RequestBody(BuildOrderBody());
            var response = await PaypalClient.Client().Execute(request); //All calls to Execute are async.

            var result = response.Result<Order>();

            return result;
        }

        /// <summary>
        /// Build the order body to sent to Paypal.
        /// </summary>
        /// <returns>A PayPalCheckoutSdk.Orders.OrderRequest</returns>
        private OrderRequest BuildOrderBody()
        {
            //Check https://developer.paypal.com/docs/api/orders/v2/ 
            
            //Here we only create the order. Note that everything is a Paypal object, therefore, 
            //if you have an object of another type, you must make the corresponding conversion.
            OrderRequest orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE", //or "AUTHORIZE"
                ApplicationContext = new ApplicationContext
                //For ApplicationContext details check: https://developer.paypal.com/docs/api/orders/v2/#definition-order_application_context
                {
                    BrandName = "Ben INC", //The label that overrides the business name in the PayPal account on the PayPal site.
                    //landing_page: the customer is redirected to a page to enter credit or debit card and other relevant billing 
                    //information required to complete the purchase.
                    LandingPage = "BILLING", //LOGIN, BILLING or NO_PREFERENCE (default) 
                    CancelUrl = "https://localhost:44322/PaypalCancel.aspx", //Goes to this page when the buyer cancels or abandons the payment.
                    ReturnUrl = "https://localhost:44322/PaypalSuccess.aspx", //It goes to this page when the buyer authorizes the payment.
                    /*
                     user_action: Configures a Continue or Pay Now checkout flow 
                    CONTINUE:  to redirect the customer to the merchant page without processing the payment.
                    PAY_NOW: no redirect to merchant page.
                     */
                    UserAction = "CONTINUE", //PAY_NOW
                    /*
                        The shipping preference:
                        Displays the shipping address to the customer. Enables the customer to choose an address on the PayPal site.
                        Restricts the customer from changing the address during the payment-approval process.

                        The possible values are:
                        GET_FROM_FILE (default). Use the customer-provided shipping address on the PayPal site.
                        NO_SHIPPING. Redact the shipping address from the PayPal site. Recommended for digital goods.
                        SET_PROVIDED_ADDRESS. Use the merchant-provided address. The customer cannot change this address on the PayPal site.                      
                     
                     */
                    ShippingPreference = "SET_PROVIDED_ADDRESS"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    //To PurchaseUnitRequest check: https://developer.paypal.com/docs/api/orders/v2/#definition-purchase_unit_request
                    new PurchaseUnitRequest{
                        ReferenceId =  "PUHF",
                        Description = "Sporting Goods",
                        CustomId = "CUST-HighFashions", //Place any information relevant to the trade here.
                        SoftDescriptor = "HighFashions",

                        #region Total amount details
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD", // Currency codes (ISO 4217)
                            Value = "220.00",                            
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "180.00"
                                },
                                Shipping = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "20.00"
                                },
                                Handling = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "10.00"
                                },
                                TaxTotal = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "20.00"
                                },
                                ShippingDiscount = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "10.00" //Positive value
                                }
                            }
                        },
                        #endregion

                        #region Detail of items to sell
                        Items = new List<Item>
                        {
                            new Item
                            {
                                Name = "T-shirt",
                                Description = "Green XL",
                                Sku = "sku01",
                                UnitAmount = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "90.00"
                                },
                                Tax = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "10.00"
                                },
                                Quantity = "1",
                                Category = "PHYSICAL_GOODS"
                            },
                            new Item
                            {
                                Name = "Shoes",
                                Description = "Running, Size 10.5",
                                Sku = "sku02",
                                UnitAmount = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "45.00"
                                },
                                Tax = new Money
                                {
                                    CurrencyCode = "USD",
                                    Value = "5.00"
                                },
                                Quantity = "2",
                                Category = "PHYSICAL_GOODS"
                            }
                        },
                        #endregion

                        #region Shipping detail info
                        ShippingDetail = new ShippingDetail // Shipping 
                        {
                            Name = new Name
                            {
                                FullName = "John Doe"
                            },
                            AddressPortable = new AddressPortable
                            {
                                AddressLine1 = "123 Townsend St",
                                AddressLine2 = "Floor 6",
                                AdminArea2 = "San Francisco",
                                AdminArea1 = "CA",
                                PostalCode = "94107",
                                CountryCode = "US"
                            }
                        }
                        #endregion
                    }
                }
            };

            return orderRequest;
        }
        #endregion

        #region Capture order
        /// <summary>
        /// Capture the order to confirm the operation to Paypal.
        /// </summary>
        /// <param name="orderId">The Order Id</param>
        /// <returns>Return a PaypalCheckoutSdk.Orders.Order</returns>
        public async Task<Order> CaptureOrder(String orderId)
        {                        
            //Get the order...
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());
            //I confirm the payment authorization from the buyer to Paypal..
            var response = await PaypalFlow.PaypalClient.Client().Execute(request);

            //Get the paypal response...
            Order result = response.Result<Order>();

            return result;
        }
        #endregion
    }
}