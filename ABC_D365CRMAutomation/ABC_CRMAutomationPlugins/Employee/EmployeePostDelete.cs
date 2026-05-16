using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace ABC_CRMAutomationPlugins.Employee
{
    public class EmployeePostDelete : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new InvalidPluginExecutionException("Deepti, here wrong in the plugin.");
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            //Message Name
            tracingService.Trace("Operation Name: {0}", context.MessageName);

            // The InputParameters collection contains all the data passed in the message request.
            if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is EntityReference deletedEmployeeRef))
            {
                tracingService.Trace("No Target or Target is not EntityReference.");
                return;
            }

            if (deletedEmployeeRef.LogicalName != "abc_employee")
            {
                tracingService.Trace("Target is not abc_employee.");
                return;
            }

            // Post-Delete: cannot retrieve record fields, only ID available
            tracingService.Trace("********** Employee Deleted **********");
            tracingService.Trace($"Employee ID: {deletedEmployeeRef.Id}");
            tracingService.Trace($"Logical Name: {deletedEmployeeRef.LogicalName}");
            tracingService.Trace("All other fields are not available in Post-Delete stage.");
            tracingService.Trace("*************************************");
        }
    }
}
