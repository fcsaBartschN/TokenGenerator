using System;
using NUnit.Framework;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased.IntegrationTests
{
    [TestFixture]
    public class BrowserBasedTokenCachingTests
    {

        [Test]
        public void TokenCacheObject_Always_CachesWhatWeSendIt()
        {
            var cacheObject = new TokenCacheObject();
            var stringToCache = "CacheThisString";
            cacheObject.SaveToCache(stringToCache);

            var retrievedString = cacheObject.LoadFromCache();

            Assert.AreEqual(stringToCache, retrievedString);
        }
        
        [Test]
        public void TokenCacheObject_WhenPassingInATempDirecotry_StoresTheTempDirectory()
        {
            var expectedCacheDirectory = "C:\\MyTempDirectory";
            var cacheObject = new TokenCacheObject(cacheDirectory: expectedCacheDirectory);

            var cacheDirectory = cacheObject.GetCacheDirectory();

            Assert.AreEqual(expectedCacheDirectory, cacheDirectory);
        }

        [Test]
        public void TokenCacheObject_WhenNotPassingInATempDirectory_SetTheTempDirectory()
        {
            var cacheObject = new TokenCacheObject();

            var cacheDirectory = cacheObject.GetCacheDirectory();

            Assert.IsNotNull(cacheDirectory);
        }

        [Test]
        public void TokenCacheObject_FromADifferentInstance_CachesWhatWeSendIt()
        {
            var cacheObject = new TokenCacheObject();
            var cacheObject2 = new TokenCacheObject();
            var stringToCache = "CacheThisString";
            cacheObject.SaveToCache(stringToCache);

            var retrievedString = cacheObject2.LoadFromCache();

            Assert.AreEqual(stringToCache, retrievedString);
        }

        [Test]
        public void TokenCacheObject_ForDifferentUsers_CachesDifferentValues()
        {
            var cacheObjectUser1 = new TokenCacheObject(user: "User1");
            var cacheObjectUser2 = new TokenCacheObject(user: "User2");
            cacheObjectUser1.SaveToCache("User1CacheString");
            cacheObjectUser2.SaveToCache("User2CacheString");

            var user1CacheString = cacheObjectUser1.LoadFromCache();
            var user2CacheString = cacheObjectUser2.LoadFromCache();

            Assert.AreNotEqual(user1CacheString, user2CacheString);
        }

        [Test]
        public void TokenCacheObject_ForDifferentPartners_CachesDifferentValues()
        {
            var cacheObjectPartner1 = new TokenCacheObject(partnerId: "Partner1");
            var cacheObjectPartner2 = new TokenCacheObject(partnerId: "Partner2");
            cacheObjectPartner1.SaveToCache("Partner1CacheString");
            cacheObjectPartner2.SaveToCache("Partner2CacheString");

            var partner1CacheString = cacheObjectPartner1.LoadFromCache();
            var partner2CacheString = cacheObjectPartner2.LoadFromCache();
            
            Assert.AreNotEqual(partner1CacheString, partner2CacheString);
        }

        [Test]
        public void TokenCacheObject_ForDifferentApplications_CachesDifferentValues()
        {
            var cacheObjectApp1 = new TokenCacheObject(applicationName: "App1");
            var cacheObjectApp2 = new TokenCacheObject(applicationName: "App2");
            cacheObjectApp1.SaveToCache("App1CacheString");
            cacheObjectApp2.SaveToCache("App2CacheString");

            var app1CacheString = cacheObjectApp1.LoadFromCache();
            var app2CacheString = cacheObjectApp2.LoadFromCache();

            Assert.AreNotEqual(app1CacheString, app2CacheString);
        }

        [Test]
        public void TokenCacheObject_ForDifferentEcsServerNames_CachesDifferentValues()
        {
            var cacheObjectEcsServer1 = new TokenCacheObject(ecsServerName: "App1");
            var cacheObjectEcsServer2 = new TokenCacheObject(ecsServerName: "App2");
            cacheObjectEcsServer1.SaveToCache("EcsServer1CacheString");
            cacheObjectEcsServer2.SaveToCache("EcsServer2CacheString");

            var ecsServer1CacheString = cacheObjectEcsServer1.LoadFromCache();
            var ecsServer2CacheString = cacheObjectEcsServer2.LoadFromCache();

            Assert.AreNotEqual(ecsServer1CacheString, ecsServer2CacheString);
        }

        [Test]
        public void ClearCache_Always_SetsTheCacheStringBackToNull()
        {
            var cacheObject = new TokenCacheObject();
            cacheObject.SaveToCache("CacheString");
            Assert.IsNotNull(cacheObject.LoadFromCache());

            cacheObject.ClearCache();

            Assert.IsNull(cacheObject.LoadFromCache());
        }

        [Test]
        public void TokenCacheObject_ForDifferentEcsSettingsButSameServer_CachesSameValue()
        {
            var cacheObjectEcsServer1Variation1 = new TokenCacheObject(ecsServerName: "http://Server1.org/variation1");
            var cacheObjectEcsServer1Variation2 = new TokenCacheObject(ecsServerName: "http://Server1.org/variation2");
            var cacheToAddress1 = "CachedToAddress1";

            cacheObjectEcsServer1Variation1.SaveToCache(cacheToAddress1);
            var loadedFromAddress2 = cacheObjectEcsServer1Variation2.LoadFromCache();
            
            Assert.AreEqual(cacheToAddress1, loadedFromAddress2);
            Console.WriteLine(cacheObjectEcsServer1Variation1.GetCacheFileName());
        }
    }
}
