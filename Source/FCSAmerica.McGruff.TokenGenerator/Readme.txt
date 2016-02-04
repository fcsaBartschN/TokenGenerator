This project shouldn't have any dependencies ( besides core .NET dependencies ), and
utilize the lowest .NET framework as possible. -curerntly v3.5.

The purpose of this project is to provide 3rd parties and non-McGruff enabled applications the
ability to generate security tokens needed to call our services.


Setup 
-------------------------------------------------------------------------------------------------
In order for this library to work, it needs to have an AuthenticationEndpoint and the AuditInfoEndpoint.   These
settings are both defined in ECS.  If you with to use the values as they are in ECS, add a reference to ECS in 
your app.config ( or web.config ) file.  

Example:
  <appSettings>
        <add key="ECSServerAddress" value="http://DevTitan.FCSAmerica.com/EnterpriseConfigurationStore/v1/RESTServices/api/ConfigItems" />
  </appSettings>

If you would like to set these two settings manually you can do so by setting the properties on the SecurityContext.  

 Example:

     var securityContext = new SecurityContext();
     securityContext.AuditInfoServiceEndpoint = "https://devinternal.fcsamerica.net/mcgruff/v2/rest/api";
     securityContext.AuthenticationEndpoint = "https://devinternal.fcsamerica.net/mcgruff/web/";


Note:  The ApplicationName and PartnerName in the AuditInfo Token will be based on the application you specify in the AuthenticationEndpoint. 
If you don't specify the AuthenticationEndpoint, then it will default to McGruff and FCSA.   In order to get the correct ApplicationName and PartnerName
in your AuditInfo you can either point the tenant specific AuthenticationEndpoint to your application's WebUI, or you can manually set the 
PartnerName and ApplicationName on the securityContext.

    Example:
	
	var securityContext = new SecurityContext();
	securityContext.PartnerName = "FCSA";
	securityContext.ApplicationName = "MyApplicationName";

	

Using
----------------------------------------------------------------
Use Case 1:  Generating a ServiceToken and AuditInfo token for the user you application is running as.

var securityContext = new SecurityContext();
var serviceToken = securityContext.ServiceToken;
var auditInfo = securityContext.AuditInfo;

Use Case 2:  Generating a ServiceToken and AuditInfo token for another user.

var securityContext = new SecurityContext(new NetworkCredential("myusername","mypassword","mydomain"));
var serviceToken = securityContext.ServiceToken;
var auditInfo = securityContext.AuditInfo;







