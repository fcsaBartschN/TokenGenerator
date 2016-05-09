using System;
using NUnit.Framework;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased.IntegrationTests
{
    [TestFixture]
    public class SecurityContextBrowserTests
    {
        [TestCase("FCSA")]
        [TestCase("NWFCS")]
        public void SSecurityContextBrowser_WithECSAddress_GetsTokenAndAuditInfo(string partnerName)
        {
            var ecsAddress = "http://devtitan.FCSAmerica.com/EnterpriseConfigurationStore/v1/RESTServices/api/ConfigItems";

            var securityContext = SecurityContextBrowser.GetInstance(ecsAddress, "DocUpDoc", partnerName, forceNewInstance: true);

            Assert.IsNotNull(securityContext);

            Assert.IsNotNull(securityContext.ServiceToken);
            Assert.IsNotNull(securityContext.AuditInfo);
            Console.WriteLine("ApplicationName:" + securityContext.ApplicationName);
            Console.WriteLine("PartnerName:" + securityContext.PartnerName);
            Console.WriteLine("ServiceToken:" + securityContext.ServiceToken);
            Console.WriteLine("AuditInfo:" + securityContext.AuditInfo);

        }

        [TestCase("FCSA")]
        [TestCase("NWFCS")]
        public void SecurityContext_WithDocuClickProxyAddress_GetTokenAndAuditInfo(string partnerName)
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v3/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var securityContext = SecurityContextBrowser.GetInstance(ecsAddress, "DocUpDoc", partnerName, forceNewInstance: true);

            Assert.IsNotNull(securityContext);


            Assert.IsNotNull(securityContext.ServiceToken);

            Console.WriteLine("ApplicationName:" + securityContext.ApplicationName);
            Console.WriteLine("PartnerName:" + securityContext.PartnerName);
            Console.WriteLine("ServiceToken:" + securityContext.ServiceToken);
            Console.WriteLine("AuditInfo:" + securityContext.AuditInfo);
        }
    }
}

