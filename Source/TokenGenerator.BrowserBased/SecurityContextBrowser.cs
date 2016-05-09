using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class SecurityContextBrowser
    {
        private readonly TraceSource _traceSource = new TraceSource("FCSAmerica.McGruff.TokenGenerator");

        private ServiceToken _serviceToken;
        private static object _lock = new object();

        private static Dictionary<string, SecurityContextBrowser> _instances;
        public static SecurityContextBrowser GetInstance(string ecsServiceAddress, string applicationName, string partnerName, bool forceNewInstance)
        {
            if (string.IsNullOrEmpty(partnerName))
            {
                throw new Exception("PartnerName cannot be null.");
            }

            if (_instances == null)
            {
                lock (_lock)
                {
                    _instances = new Dictionary<string, SecurityContextBrowser>();
                }
            }

            lock (_lock)
            {
                if (!_instances.ContainsKey(partnerName) || forceNewInstance)
                {
                    _instances[partnerName] = new SecurityContextBrowser(ecsServiceAddress, applicationName, partnerName);
                }
            }
            return _instances[partnerName];
        }

        public static SecurityContextBrowser GetInstance(string applicationName, string partnerName, bool forceNewInstance)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, forceNewInstance);
        }

        public static SecurityContextBrowser GetInstance(string applicationName, string partnerName)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, false);
        }


        private SecurityContextBrowser(string ecsServiceAddress, string applicationName, string partnerName)
        {
            _traceSource.TraceInformation("\nInitialied PartnerClientSecurityToken for partner: ", partnerName);

            _serviceToken = new BrowserBasedServiceToken(ecsServiceAddress, applicationName, partnerName);

        }

        public string AuthenticationEndpoint
        {
            get { return _serviceToken.AuthenticationEndpoint; }
            set { _serviceToken.AuthenticationEndpoint = value; }
        }

        public string AuditInfoServiceEndpoint
        {
            get { return _serviceToken.AuditInfoServiceEndpoint; }
            set { _serviceToken.AuditInfoServiceEndpoint = value; }
        }

        public string RelyingParty
        {
            get { return _serviceToken.RelyingParty; }
            set { _serviceToken.RelyingParty = value; }
        }

        public string IdentityProvider
        {
            get { return _serviceToken.IdentityProvider; }
            set { _serviceToken.IdentityProvider = value; }
        }

        public string IdpTokenOverride
        {
            get { return _serviceToken.IdpTokenOverride; }
            set { _serviceToken.IdpTokenOverride = value; }
        }

        public int RefreshMinutesBeforeExpire
        {
            get { return _serviceToken.RefreshMinutesBeforeExpire; }
            set { _serviceToken.RefreshMinutesBeforeExpire = value; }
        }

        public string PartnerName
        {
            get { return _serviceToken.PartnerName; }
            set
            {
                if (_serviceToken.PartnerName != value)
                {
                    _serviceToken.AuditInfo = null; // force it to refresh.
                    _serviceToken.PartnerName = value;
                }
            }
        }

        public string ApplicationName
        {
            get { return _serviceToken.ApplicationName; }
            set { _serviceToken.ApplicationName = value; }
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
