using System;
using System.Diagnostics;
using System.Threading;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{

    public class BrowserBasedServiceToken : ServiceToken
    {
        private readonly ITokenCache _cache;
        private readonly TraceSource _traceSource = new TraceSource("FCSAmerica.McGruff.TokenGenerator");

        private const int DefaultTimeoutInMiliSeconds = 30000;


        public BrowserBasedServiceToken(string ecsServiceAddress, string applicationName, string partnerName, ITokenCache cache = null)
            : base(ecsServiceAddress, applicationName, partnerName)
        {
            _cache = cache;
        }

        protected override string RefreshAuditInfo()
        {
            try
            {
                _auditInfo = _cache?.LoadAuditInfoFromCache();
                if (_auditInfo != null)
                {
                    return _auditInfo;
                }
            }
            catch
            {
                _token = null;
                _auditInfo = null;
                _cache?.ClearCache();
            }

            _auditInfo = base.RefreshAuditInfo();
            _cache?.SaveAuditInfoToCache(_auditInfo);

            return _auditInfo;
        }

        protected override void RefreshToken()
        {
            _traceSource.TraceInformation("\nStarted using BrowserBasedTokenRetriever using {0}.", AuthenticationEndpoint);

            try
            {
                _token = _cache?.LoadFromCache();
                if (_token != null)
                {
                    SetExpireDateFromToken();
                }
            }
            catch
            {
                _token = null;
                _auditInfo = null;
                _cache?.ClearCache();
            }

            if (string.IsNullOrEmpty(_token) || IsExpiredOrAboutToExpire)
            {
                _cache?.ClearCache();
                try
                {
                    string stsToken = null;
                    string issuingAuthority = ConfigItems["v2.FcsaIssuingAuthority"];

                    var securityContextRetreiver = new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        using (var t = new BrowserTokenRetriever(AuthenticationEndpoint, issuingAuthority))
                        {
                            stsToken = t.RetrieveToken();
                        }
                    });
                    securityContextRetreiver.SetApartmentState(ApartmentState.STA);
                    securityContextRetreiver.Start();
                    securityContextRetreiver.Join(DefaultTimeoutInMiliSeconds);

                    _token = !string.IsNullOrEmpty(stsToken) ? CleanToken(stsToken) : stsToken;
                    _cache?.SaveToCache(_token);

                    _traceSource.TraceInformation("\nCompleted retrieving token from BrowerBasedTokenRetriever.");
                }
                catch (Exception ex)
                {
                    _traceSource.TraceEvent(TraceEventType.Error, 0,
                        "\nException occured during TokenRetriever RetrieveToken.\n" + UnwrapException(ex).ToString());
                }

                SetExpireDateFromToken();
            }
        }

        private Exception UnwrapException(Exception exception)
        {
            AggregateException aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                return aggregateException.Flatten().InnerException;
            }
            return exception;
        }
    }
}
