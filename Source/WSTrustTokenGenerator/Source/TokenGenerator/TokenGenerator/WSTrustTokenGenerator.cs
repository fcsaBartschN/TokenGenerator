using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using Thinktecture.IdentityModel.WSTrust;


namespace TokenGenerator
{
    public class WSTrustTokenGenerator
    {
        //https://leastprivilege.com/2010/10/28/wif-adfs-2-and-wcfpart-6-chaining-multiple-token-services/
        //https://msdn.microsoft.com/en-us/library/ee517297.aspx

        public SecurityToken GetToken(string idpEndpoint, string rstsRealm, string userName, string password)
        {
           
           var binding = new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential);

            var factory = new System.ServiceModel.Security.WSTrustChannelFactory(binding, new EndpointAddress(new Uri(idpEndpoint)));
            
            factory.TrustVersion = TrustVersion.WSTrust13;
            factory.Credentials.SupportInteractive = false;
            factory.Credentials.UserName.UserName = userName;
            factory.Credentials.UserName.Password = password;

            
            var rst = new System.IdentityModel.Protocols.WSTrust.RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new System.IdentityModel.Protocols.WSTrust.EndpointReference(rstsRealm),
                KeyType = KeyTypes.Bearer,
                TokenType = "urn:oasis:names:tc:SAML:2.0:assertion"
            };

            var channel = factory.CreateChannel();
            var securityToken = channel.Issue(rst);
            return securityToken;
        }

        public string GetToken(string idpEndpoint, string rstsRealm)
        {
            var binding = new WindowsWSTrustBinding(SecurityMode.TransportWithMessageCredential);

            var factory = new System.ServiceModel.Security.WSTrustChannelFactory(binding, new EndpointAddress(new Uri(idpEndpoint)));
          
            factory.TrustVersion = TrustVersion.WSTrust13;
            factory.Credentials.SupportInteractive = false;

            var rst = new System.IdentityModel.Protocols.WSTrust.RequestSecurityToken
            {
                RequestType = RequestTypes.Issue,
                AppliesTo = new System.IdentityModel.Protocols.WSTrust.EndpointReference(rstsRealm),
                KeyType = KeyTypes.Bearer,
                TokenType = "urn:oasis:names:tc:SAML:1.0:assertion" // "urn:oasis:names:tc:SAML:2.0:assertion" 
            };

            var channel = factory.CreateChannel();
            RequestSecurityTokenResponse response = null;
            try {
                var securityToken = channel.Issue(rst, out response);
                return Serialize(response);
            }catch
            {
                var x = response;
            }
            return null;
        
        }

        public string Serialize(RequestSecurityTokenResponse securityTokenResponse)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.OmitXmlDeclaration = true;

            using (MemoryStream memoryStream = new MemoryStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                WSTrust13ResponseSerializer serializer = new WSTrust13ResponseSerializer();
                WSTrustSerializationContext context = new WSTrustSerializationContext();
                serializer.WriteXml(securityTokenResponse, xmlWriter, context);
                xmlWriter.Flush();
                var encoding = new System.Text.UTF8Encoding(false);
                return encoding.GetString(memoryStream.ToArray());

            }
        }
      

    }
}


