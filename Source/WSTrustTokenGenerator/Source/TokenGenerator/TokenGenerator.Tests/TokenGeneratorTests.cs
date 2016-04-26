using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TokenGenerator.Tests
{
    [TestClass]
    public class TokenGeneratorTests
    {
        [TestMethod]
        public void TestMethod1()   
        {
            var tokenGenerator = new TokenGenerator();
            // var token = tokenGenerator.GetIdPToken("https://testfs.fcsamerica.com/adfs/services/trust/13/usernamemixed", "https://teststs.fcsamerica.net/", "FCSAmerica\\username", "password" );

            var token = tokenGenerator.GetIdPToken("https://testfs.fcsamerica.com/adfs/services/trust/13/windowsmixed", "https://teststs.fcsamerica.net/");
            var stsToken = tokenGenerator.GetSTSToken(token, "https://teststs.fcsamerica.net/", "devfcma.fcsamerica.net:AGL");
        }
    }
}
