using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class BrowserTokenRetriever : IDisposable
    {
        private readonly TraceSource _traceSource = new TraceSource("FCSAmerica.McGruff.TokenGenerator");

        private const int MaxRedirectLoopCount = 10;

        private readonly string _authenticationUrl;
        private readonly string _issuingAuthority;

        private int _browserRedirectCount;

        private readonly WebBrowser _browser;

        private string _stsToken;


        public BrowserTokenRetriever(string authenticationUrl, string issuingAuthority)
        {
            _authenticationUrl = authenticationUrl;
            _issuingAuthority = issuingAuthority;

            _browser = new WebBrowser();
            _browser.Visible = false;
            _browser.AllowNavigation = true;
            _browser.Navigated += _browser_Navigated;
            _browser.DocumentCompleted += _browser_DocumentCompleted;
        }

        public string RetrieveToken()
        {
            _browser.Navigate(_authenticationUrl);

            while (_stsToken == null)
            {
                Application.DoEvents();
            }

            return _stsToken;
        }

        void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                _browserRedirectCount++;

                var currentBrowser = (sender as WebBrowser);
                if (currentBrowser == null)
                {
                    RaiseTokenRetrievalError(new Exception("WebBrowser is null."));
                    return;
                }

                var document = currentBrowser.Document;

                if (document == null)
                {
                    RaiseTokenRetrievalError(new Exception("document is null."));
                    return;
                }
                _traceSource.TraceInformation("\nBrowser_DocumentComplete Count: " + _browserRedirectCount);

                //_traceSource.TraceInformation("\n\nHtml:\n{0}\n", (document.body == null) ? document.toString() : document.body.innerHTML);

                if (document.ActiveElement != null)
                {
                    var wsResultElement = document.All.GetElementsByName("wresult");
                    if (wsResultElement != null && wsResultElement.Count > 0)
                    {

                        var wsResultValue = wsResultElement[0].GetAttribute("value");

                        if (wsResultValue != null)
                        {
                            var issuerRedirectUrlSearchText = string.Format("<Issuer>{0}</Issuer>", _issuingAuthority);

                            if (wsResultValue.IndexOf(issuerRedirectUrlSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // stop we have the token.
                                var stsToken = wsResultValue;
                                RaiseTokenRetrievalCompletion(stsToken);
                            }
                        }
                    }
                }

                if (_browserRedirectCount == MaxRedirectLoopCount)
                {
                    _traceSource.TraceInformation("\nMaxRedirectLoopCount reached. Stopping browser and returning back empty token.");

                    //Raise TokenRetrievalCompletion with empty token
                    RaiseTokenRetrievalCompletion(string.Empty);
                }
            }
            catch (Exception ex)
            {
                RaiseTokenRetrievalError(ex);
            }
        }

        void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            _traceSource.TraceInformation("\nBrowser Navigation completed to url: {0}", e.Url);
        }

        private void RaiseTokenRetrievalCompletion(string token)
        {
            _stsToken = token;
        }

        private void RaiseTokenRetrievalError(Exception ex)
        {
            _traceSource.TraceEvent(TraceEventType.Error, 0, "Error Occured during DocumentComplete.\n{0}", ex.ToString());

            _stsToken = string.Empty;
        }

        public void Dispose()
        {
            if (_browser != null)
            {
                _browser.Dispose();
            }
        }
    }
}
