using System;
using System.Diagnostics;
using System.Threading.Tasks;
using mshtml;
using SHDocVw;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class BrowserTokenRetriever : IDisposable
    {
        private readonly TraceSource _traceSource = new TraceSource("FCSAmerica.McGruff.TokenGenerator");

        private const int MaxRedirectLoopCount = 10;

        private readonly string _authenticationUrl;
        private readonly string _issuingAuthority;

        private int _browserRedirectCount;

        private readonly InternetExplorer _browser;

        private TaskCompletionSource<string> _taskCompletionSource;

        public event EventHandler<TokenRetrievedEventArgs> TokenRetrievalCompletion;

        public event EventHandler<Exception> TokenRetrievalError;
        
        public BrowserTokenRetriever(string authenticationUrl, string issuingAuthority)
        {
            _authenticationUrl = authenticationUrl;
            _issuingAuthority = issuingAuthority;

            _browser = new InternetExplorer();
            _browser.Visible = false;
            _browser.NavigateComplete2 += _browser_NavigateComplete2;
            _browser.DocumentComplete += browser_DocumentComplete;
            _browser.NavigateError += browser_NavigateError;

            TokenRetrievalError += TokenRetriever_TokenRetrievalError;
            TokenRetrievalCompletion += TokenRetriever_TokenRetrievalCompletion;
        }

        public Task<string> RetrieveToken()
        {
            _taskCompletionSource = new TaskCompletionSource<string>();

            try
            {
                _browser.Navigate2(_authenticationUrl);
            }
            catch (Exception ex)
            {
                RaiseTokenRetrievalError(ex);
            }
            return _taskCompletionSource.Task;
        }

        void browser_DocumentComplete(object pDisp, ref object url)
        {
            try
            {
                _browserRedirectCount++;

                var currentBrowser = (pDisp as WebBrowser);
                if (currentBrowser == null)
                {
                    RaiseTokenRetrievalError(new Exception("WebBrowser is null."));
                    return;
                }

                var document = currentBrowser.Document as HTMLDocumentClass;

                if (document == null)
                {
                    RaiseTokenRetrievalError(new Exception("document is null."));
                    return;
                }
                _traceSource.TraceInformation("\nBrowser_DocumentComplete Count: " + _browserRedirectCount);
                
                //_traceSource.TraceInformation("\n\nHtml:\n{0}\n", (document.body == null) ? document.toString() : document.body.innerHTML);

                if (document.activeElement != null)
                {
                    var wsResultElement = document.getElementsByName("wresult");
                    if (wsResultElement != null)
                    {

                        var wsResultHtmlElement = wsResultElement.item(0) as HTMLInputElement;

                        if (wsResultHtmlElement != null)
                        {
                            var issuerRedirectUrlSearchText = string.Format("<Issuer>{0}</Issuer>", _issuingAuthority);

                            if (wsResultHtmlElement.defaultValue.IndexOf(issuerRedirectUrlSearchText, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // stop we have the token.
                                var stsToken = wsResultHtmlElement.value;
                                RaiseTokenRetrievalCompletion(new TokenRetrievedEventArgs { Token = stsToken });
                            }
                        }
                    }
                }

                if (_browserRedirectCount == MaxRedirectLoopCount)
                {
                    _traceSource.TraceInformation("\nMaxRedirectLoopCount reached. Stopping browser and returning back empty token.");

                    //Raise TokenRetrievalCompletion with empty token
                    RaiseTokenRetrievalCompletion(new TokenRetrievedEventArgs { Token = null });
                }
            }
            catch (Exception ex)
            {
                RaiseTokenRetrievalError(ex);
                _traceSource.TraceEvent(TraceEventType.Error, 0, "Error Occured during DocumentComplete.\n{0}", ex.ToString());
            }
        }

        void browser_NavigateError(object pDisp, ref object url, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            var x = pDisp;
            var y = StatusCode;
            Cancel = true;
            RaiseTokenRetrievalError(new Exception("Navigation error at url: " + url));
        }
        void _browser_NavigateComplete2(object pDisp, ref object URL)
        {

            var x = pDisp;
            _traceSource.TraceInformation("\nBrowser Navigation completed to url: {0}", URL);
        }

        private void QuitBrowser()
        {
            if (_browser != null)
            {
                _browser.Quit();
            }
        }
        
        private void RaiseTokenRetrievalCompletion(TokenRetrievedEventArgs e)
        {
            EventHandler<TokenRetrievedEventArgs> handler = TokenRetrievalCompletion;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseTokenRetrievalError(Exception ex)
        {
            EventHandler<Exception> handler = TokenRetrievalError;
            if (handler != null)
            {
                handler(this, ex);
            }
        }

        void TokenRetriever_TokenRetrievalCompletion(object sender, TokenRetrievedEventArgs e)
        {
            QuitBrowser();

            if (_taskCompletionSource != null)
            {
                _taskCompletionSource.TrySetResult(e.Token);
            }

        }

        void TokenRetriever_TokenRetrievalError(object sender, Exception ex)
        {
            QuitBrowser();

            if (_taskCompletionSource != null)
            {
                _taskCompletionSource.SetException(ex);
            }
        }

        public void Dispose()
        {
            QuitBrowser();
        }
    }
}
