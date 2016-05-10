The purpose of this project is to provide partner client application to generate security tokens needed to call our services.


Setup 
-------------------------------------------------------------------------------------------------
In order for this library to work, it needs to have an AuthenticationEndpoint and the AuditInfoEndpoint.   These
settings are both defined in ECS. If you with to use the values as they are in ECS, add a "ECSServerAddress" settings in 
your app.config ( or web.config ) file.  

Example:
  <appSettings>
        <add key="ECSServerAddress" value="http://DevTitan.FCSAmerica.com/EnterpriseConfigurationStore/v1/RESTServices/api/ConfigItems" />
  </appSettings>

  If using For Partner specific application, use proxy as below if u have created
  
  <appSettings>
        <add key="ECSServerAddress" value="https://devinternal.fcsamerica.net/DocuClick/v3/REST/api/Proxy/EnterpriseConfigurationStore/v1/ConfigItems" />
  </appSettings>

  

Note:  The ApplicationName and PartnerName in the AuditInfo Token will be based on the application you specify. 

    Example:
	
	var securityContext = BrowserBasedSecurityContext.GetInstance(ecsAddress, applicationName, partnerName, forceNewInstance: false);
	var serviceToken = securityContext.ServiceToken;
	var auditInfo = securityContext.AuditInfo;

	

Using
----------------------------------------------------------------
Use Case 1:  Generating a ServiceToken and AuditInfo token for the partner client application or service running if Ecsaddress in config.

var securityContext = var securityContext = BrowserBasedSecurityContext.GetInstance(applicationName, partnerName);
var serviceToken = securityContext.ServiceToken;
var auditInfo = securityContext.AuditInfo;
