using System;

namespace FCSAmerica.McGruff.TokenGenerator.BrowserBased
{
    public class TokenRetrievedEventArgs : EventArgs
    {
        public string Token { get; set; }
    }
}
