using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace FCSAmerica.McGruff.TokenGenerator
{
    public class SecurityContext
    {
        private ServiceToken _serviceToken;
        private static object _lock = new object();

        public static Dictionary<string, SecurityContext> _instances;
        public static SecurityContext GetInstance(string ecsServiceAddress, string applicationName, string partnerName, bool forceNewInstance)
        {
            if(_instances == null)
            {   
                lock(_lock)
                {
                    _instances = new Dictionary<string, SecurityContext>();
                }
            }

            lock (_lock)
            {
                if (!_instances.ContainsKey(partnerName) || forceNewInstance)
                {
                    _instances[partnerName] = new SecurityContext(ecsServiceAddress, applicationName, partnerName);
                }
            }
            return _instances[partnerName];
        }

        public static SecurityContext GetInstance(string applicationName, string partnerName, bool forceNewInstance)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, forceNewInstance);
        }

        public static SecurityContext GetInstance(string applicationName, string partnerName)
        {
            return GetInstance(ConfigurationManager.AppSettings["ECSServerAddress"], applicationName, partnerName, false);
        }



        private SecurityContext(string ecsServiceAddress, string applicationName, string partnerName )
        {
            _serviceToken = new ServiceToken(ecsServiceAddress, applicationName, partnerName);
        }

        public SecurityContext(string ecsServiceAddress, NetworkCredential credential, string applicationName, string partnerName)
        {
            _serviceToken = new ServiceToken(ecsServiceAddress, credential, applicationName, partnerName);
        }

        public SecurityContext(NetworkCredential credential, string applicationName, string partnerName)
        {
            string ecsServiceAddress = ConfigurationManager.AppSettings["ECSServerAddress"]; // can be null.
            _serviceToken = new ServiceToken(ecsServiceAddress, credential, applicationName, partnerName );
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
            set {
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
