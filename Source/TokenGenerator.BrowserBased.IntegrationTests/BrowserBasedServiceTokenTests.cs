using System;
using NUnit.Framework;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased.IntegrationTests
{
    [TestFixture]
    public class BrowserBasedServiceTokenTests
    {
        [Test]
        public void BrowserBasedServiceToken_Defaults_ExpirationTimeToMaxValue()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v3/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA");

            var tokenExpireDate = browserBasedToken.TokenExpireDate;

            Console.WriteLine(tokenExpireDate);
            Assert.AreEqual(DateTime.MaxValue, tokenExpireDate);
        }

        [Test]
        public void BrowserBasedServiceToken_AfterTokenRetrieval_SetsTheExpirationTime()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v3/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA");

            // Retrieve the token; this should set expiration date to something other than max value
            var token = browserBasedToken.Token;

            var tokenExpireDate = browserBasedToken.TokenExpireDate;
            Console.WriteLine(tokenExpireDate);
            Assert.IsTrue(tokenExpireDate < DateTime.MaxValue);
        }
    }
}

