using PayPalCheckoutSdk.Core;
using PayPalHttp;
using System;
using System.Configuration;
using System.Web;

namespace Paypal_FW461.Shared
{
    public class PageBase : System.Web.UI.Page
    {
        /// <summary>
        /// Simulates saving information in the database. 
        /// </summary>
        /// <remarks>
        /// In your database, dont forget create the relation between {Paypal Order Id} and your own {Order Id}.
        /// </remarks>
        /// <param name="key">The key in the appSettings section (Settings.config)</param>
        /// <param name="value">The value to save.</param>
        public void SaveInDatabase(String key, String value)
        {
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~"); ;
            //if (HttpContext.Current != null)
            //{
            //    config =
            //        System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //}
            //else
            //{
            //    config =
            //        ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //}
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}