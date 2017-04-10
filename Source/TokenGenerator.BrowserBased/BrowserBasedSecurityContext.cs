using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class BrowserBasedSecurityContext
    {
        private readonly TraceSource _traceSource = new TraceSource("FCSAmerica.McGruff.TokenGenerator");

        private readonly ServiceToken _serviceToken;
        private static readonly object Lock = new object();

        private static Dictionary<string, BrowserBasedSecurityContext> instances;
        public static BrowserBasedSecurityContext GetInstance(string ecsServiceAddress, string applicationName, string partnerName, bool forceNewInstance)
        {
            if (string.IsNullOrEmpty(partnerName))
            {
                throw new Exception("PartnerName cannot be null.");
            }

            if (instances == null)
            {
                lock (Lock)
                {
                    instances = new Dictionary<string, BrowserBasedSecurityContext>();
                }
            }

            lock (Lock)
            {
                if (!instances.ContainsKey(partnerName) || forceNewInstance)
                {
                    TokenCacheObject tokenCache = new TokenCacheObject(Environment.UserName, partnerName, applicationName, ecsServiceAddress);
                    if (forceNewInstance)
                    {
                        tokenCache.ClearCache();
                    }
                    instances[partnerName] = new BrowserBasedSecurityContext(ecsServiceAddress, applicationName, partnerName, tokenCache);
                }
            }
            return instances[partnerName];
        }

        public static BrowserBasedSecurityContext GetInstance(string applicationName, string partnerName, bool forceNewInstance)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, forceNewInstance);
        }

        public static BrowserBasedSecurityContext GetInstance(string applicationName, string partnerName)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, false);
        }


        private BrowserBasedSecurityContext(string ecsServiceAddress, string applicationName, string partnerName, ITokenCache tokenCache)
        {
            _traceSource.TraceInformation("\nInitialized PartnerClientSecurityToken for partner: ", partnerName);
            _serviceToken = new BrowserBasedServiceToken(ecsServiceAddress, applicationName, partnerName, tokenCache);

        }
        
        public string PartnerName
        {
            get { return _serviceToken.PartnerName; }
            
        }

        public string ApplicationName
        {
            get { return _serviceToken.ApplicationName; }
        }


        public string ServiceToken
        {
            get { return "SAML " + _serviceToken.Token; }
        }

        public string AuditInfo
        {
            get
            {
                return _serviceToken.AuditInfo;
            }
        }
    }
}
