using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FakeItEasy;
using NUnit.Framework;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased.IntegrationTests
{
    [TestFixture]
    public class BrowserBasedServiceTokenTests
    {
        [Test]
        public void BrowserBasedServiceToken_Defaults_ExpirationTimeToMaxValue()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA");

            var tokenExpireDate = browserBasedToken.TokenExpireDate;

            Console.WriteLine(tokenExpireDate);
            Assert.AreEqual(DateTime.MaxValue, tokenExpireDate);
        }

        [Test]
        public void BrowserBasedServiceToken_AfterTokenRetrieval_SetsTheExpirationTime()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA");

            // Retrieve the token; this should set expiration date to something other than max value
            var token = browserBasedToken.Token;

            var tokenExpireDate = browserBasedToken.TokenExpireDate;
            Console.WriteLine(tokenExpireDate);
            Assert.IsTrue(tokenExpireDate < DateTime.MaxValue);
        }

        [Test]
        public void BrowserBasedServiceToken_Always_AllowsCreationWithACacheObject()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>();

            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTheCachedTokenIsNull_SaveTheTokenToTheCache()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>();
            A.CallTo(() => cache.LoadFromCache()).Returns(null);
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            // Retrieve the token; this should set expiration date to something other than max value
            var token = browserBasedToken.Token;
            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);

            var tokenExpireDate = browserBasedToken.TokenExpireDate;
            Console.WriteLine(tokenExpireDate);
            Assert.IsTrue(tokenExpireDate < DateTime.MaxValue);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTokenIsCached_ReturnsTheCachedToken()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>(x => x.Strict());
            A.CallTo(() => cache.LoadFromCache()).Returns("TOKEN");
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            byte[] bytesToEncode = Encoding.UTF8.GetBytes("TOKEN");
            var encodedToken = Convert.ToBase64String(bytesToEncode);
            Assert.AreEqual(encodedToken, token);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTokenIsCached_DoesntCacheTheToken()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>(x => x.Strict());
            A.CallTo(() => cache.LoadFromCache()).Returns("TOKEN");
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void BrowserBasedServiceToken_WhenSaveThrowsAnError_ClearsTheCache()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>();
            A.CallTo(() => cache.LoadFromCache()).Returns(null);
            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).Throws<Exception>();
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            A.CallTo(() => cache.ClearCache()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}

