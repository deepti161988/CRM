using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;


namespace ABC_CRMAutomationPlugins.Employee
{
    public class EmployeePostCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new InvalidPluginExecutionException("Deepti, here wrong in the plugin.");
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory =
    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                serviceFactory.CreateOrganizationService(context.UserId);

            //Message Name
            tracingService.Trace("Operation Name: {0}", context.MessageName);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "abc_employee")
                    return;

                //tracingService.Trace("Post Operation: Updating Gender and Address");
                //tracingService.Trace("Post Operation: Updating Gender and Address Deepti");

                //Entity account = new Entity("account");

                //account["name"] = "Sample Account from Plugin1";
                //account["telephone1"] = "12345678901";
                //account["address1_city"] = "Pune1";

                //// Create record in CRM
                //Guid accountId = service.Create(account);

                //tracingService.Trace("Account created with ID: {0}", accountId);
                try
                {
                    tracingService.Trace("Post Operation: Sending Email to Admin");


                    // =========Email Creation ====================
                    string adminEmail = "admin@CRM827909.onmicrosoft.com";
                    Entity fromParty = new Entity("activityparty");
                    fromParty["partyid"] = new EntityReference("systemuser", context.UserId);
                    Entity toParty = new Entity("activityparty");
                    toParty["addressused"] = adminEmail;

                    var ccParty = new Entity("activityparty");
                    ccParty["partyid"] = new EntityReference("systemuser", context.UserId);
                                      

                    Entity emailEntity = new Entity("email");
                    emailEntity["from"] = new EntityCollection(new[] { fromParty });
                    emailEntity["to"] = new EntityCollection(new[] { toParty });
                    emailEntity["cc"] = new EntityCollection(new[] { ccParty });
                    emailEntity["bcc"] = new EntityCollection(new[] { ccParty });
                    emailEntity["subject"] = "New Employee Created";
                    emailEntity["description"] = "A new employee record has been created in CRM.";
                    emailEntity["regardingobjectid"] = new EntityReference("abc_employee", context.PrimaryEntityId);
                    
                    Guid emailId = service.Create(emailEntity);
                    //===================Attachment txtx File to the Above Email ====================

                    string fileContent = "This is sample content of abc.txt file";
                    byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);
                    string base64File = Convert.ToBase64String(fileBytes);
                    Entity attachment = new Entity("activitymimeattachment");
                    attachment["objectid"] = new EntityReference("email", emailId);
                    attachment["objecttypecode"] = "email";
                    attachment["subject"] = "Text File Attachment";
                    attachment["filename"] = "abc.txt";
                    attachment["body"] = base64File;
                    attachment["mimetype"] = "text/plain";

                    service.Create(attachment);

                    //==========Send Email========================

                    tracingService.Trace("Email created with ID: {0}", emailId);
                    // 🔹 Send Email
                    var sendEmailRequest = new OrganizationRequest("SendEmail");
                    sendEmailRequest["EmailId"] = emailId;
                    sendEmailRequest["TrackingToken"] = "";
                    sendEmailRequest["IssueSend"] = true;
                    service.Execute(sendEmailRequest);
                    tracingService.Trace("Email sent successfully to Admin.");
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Error: {0}", ex.ToString());
                    throw;
                }
            }

        }
    }
}

