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
            A.CallTo(() => cache.ClearCache()).DoesNothing();
            var utcNow = DateTimeOffset.Now;
            var utcOneHourFromNow = utcNow.AddHours(1);
            var tokenText = $"<Conditions NotBefore=\"{utcNow}\" NotOnOrAfter=\"{utcOneHourFromNow}\"></Conditions>";

            A.CallTo(() => cache.LoadFromCache()).Returns(tokenText);
            A.CallTo(() => cache.LoadAuditInfoFromCache()).Returns(null);
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            byte[] bytesToEncode = Encoding.UTF8.GetBytes(tokenText);
            var encodedToken = Convert.ToBase64String(bytesToEncode);
            Assert.AreEqual(encodedToken, token);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTokenIsCached_DoesntSaveTheTokenAgain()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>(x => x.Strict());
            var utcNow = DateTimeOffset.Now;
            var utcOneHourFromNow = utcNow.AddHours(1);

            A.CallTo(() => cache.LoadFromCache()).Returns($"<Conditions NotBefore=\"{utcNow}\" NotOnOrAfter=\"{utcOneHourFromNow}\" ></Conditions>");
            A.CallTo(() => cache.LoadAuditInfoFromCache()).Returns(null);
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
            A.CallTo(() => cache.ClearCache()).DoesNothing();
            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).Throws<Exception>();
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            A.CallTo(() => cache.ClearCache()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTokenIsInvalid_ClearsTheCache()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>();
            A.CallTo(() => cache.LoadFromCache()).Returns("INVALID XML");
            A.CallTo(() => cache.ClearCache()).DoesNothing();
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            A.CallTo(() => cache.ClearCache()).MustHaveHappened(Repeated.AtLeast.Once);
            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void BrowserBasedServiceToken_WhenTokenIsCachedAndOld_RetrievesANewTokenAndSavesIt()
        {
            var ecsAddress = "https://devinternal.fcsamerica.net/DocuClick/v4/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems/";
            var cache = A.Fake<ITokenCache>(x => x.Strict());
            A.CallTo(() => cache.ClearCache()).DoesNothing();
            var utcOneHourAgo = DateTimeOffset.Now.AddHours(-1);
            var utcTwoHourAgo = utcOneHourAgo.AddHours(-1);

            A.CallTo(() => cache.LoadFromCache()).Returns($"<Conditions NotBefore=\"{utcTwoHourAgo}\" NotOnOrAfter=\"{utcOneHourAgo}\">");
            var browserBasedToken = new BrowserBasedServiceToken(ecsAddress, "DocIndexer", "FCSA", cache);

            var token = browserBasedToken.Token;

            A.CallTo(() => cache.SaveToCache(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => cache.ClearCache()).MustHaveHappened();
        }
    }
}

