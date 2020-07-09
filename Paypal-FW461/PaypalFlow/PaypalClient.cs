using System;
using PayPalCheckoutSdk.Core;
using PayPalHttp;
using System.Configuration;

namespace Paypal_FW461.PaypalFlow
{
    public class PaypalClient
    {
        /// <summary>
        /// Setting credentials.
        /// PayPalHttpClient and environment create the auth token to paypal API REST
        /// </summary>
        /// <remarks>
        /// HttpClient is a 'PayPalHttp' not a 'System.Net.Http'
        /// </remarks>
        /// <returns>Returns PayPalHttpClient instance which can be used to invoke PayPal API's.</returns>
        public static HttpClient Client()
        {
            // Creating a sandbox environment
            // In production: Replace SandboxEnviroment to LiveEnvironment
            PayPalEnvironment environment = new SandboxEnvironment(
                ConfigurationManager.AppSettings["PaypalClientId"],
                ConfigurationManager.AppSettings["PaypalClientSecret"]);

            // Creating a client for the environment
            // Returns PayPalHttpClient instance to invoke PayPal APIs.
            PayPalHttpClient client = new PayPalHttpClient(environment);
            return client;
        }
    }
}