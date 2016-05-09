using NUnit.Framework;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased.IntegrationTests
{
    [TestFixture]
    public class ServiceTokenTests
    {
        
        [TestCase("FCSA","https://devinternal.fcsamerica.net/mcgruff/AdminUI/")]
        [TestCase("NWFCS", "https://devnwfcs.fcsamerica.net/mcgruff/reference/web")]
        public void SecurityContext_WithECSAddress_GetsTokenAndAuditInfo(string partnerName, string authenticationEndpoint)
        {
            var ecsAddress = "http://devtitan.FCSAmerica.com/EnterpriseConfigurationStore/v1/RESTServices/api/ConfigItems";
            var serviceToken = new ServiceToken(ecsAddress, "McGruff", partnerName);
            var configItems = serviceToken.ConfigItems;
            Assert.IsNotNull(configItems);
            
            Assert.AreEqual(authenticationEndpoint,configItems["v2.AuthenticationEndpoint"]);
        }
    }
}

