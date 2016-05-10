using System;
using System.Configuration;
using System.Text;
using FCSAmerica.McGruff.TokenGenerator.BrowserBased;

namespace TokenGenerator.BrowserBased.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ecsAddress = ConfigurationManager.AppSettings["ECSServerAddress"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var partnerName = ConfigurationManager.AppSettings["PartnerName"];

            var securityContext = BrowserBasedSecurityContext.GetInstance(ecsAddress, applicationName, partnerName, forceNewInstance: false);

            Console.WriteLine("ApplicationName:" + securityContext.ApplicationName);
            Console.WriteLine("PartnerName:" + securityContext.PartnerName);
            Console.WriteLine("\nServiceToken:" + securityContext.ServiceToken);
            Console.WriteLine("\nAuditInfo:" + securityContext.AuditInfo);
            Console.WriteLine("\nDecoded AuditInfo:" + DecodedBase64(securityContext.AuditInfo));


            Console.ReadLine();
        }

        private static string DecodedBase64(string base64String)
        {
            byte[] data = Convert.FromBase64String(base64String);

            string decodedString = Encoding.UTF8.GetString(data);

            return decodedString;
        }
    }
}
