using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace ABC_CRMAutomationPlugins.Employee
{
    public class EmployeePreUpdate : IPlugin
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


        }
    }
}
