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
    public class EmployeePreCreateDuplicateDetect : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new InvalidPluginExecutionException("Deepti, here wrong in the plugin.");
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Message Name
            tracingService.Trace("Operation Name: {0}", context.MessageName);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                // Only run for abc_employee entity
                if (entity.LogicalName != "abc_employee")
                    return;

                // Check if employeename field exists
                if (!entity.Attributes.Contains("abc_employeename"))
                    return; // No name provided, skip validation

                string employeeName = entity.GetAttributeValue<string>("abc_employeename").Trim();

                tracingService.Trace("Checking duplicate for employee name: {0}", employeeName);

                // Get organization service
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                // Query to check duplicates
                QueryExpression query = new QueryExpression("abc_employee");
                query.ColumnSet = new ColumnSet("abc_employeename");
                query.Criteria.AddCondition("abc_employeename", ConditionOperator.Equal, employeeName);

                EntityCollection existingEmployees = service.RetrieveMultiple(query);

                if (existingEmployees.Entities.Count > 0)
                {
                    // Duplicate found → throw exception to block creation
                    throw new InvalidPluginExecutionException($"An employee with the name '{employeeName}' already exists.");
                }

                tracingService.Trace("No duplicate found, record can be created.");
            }
        }
    }

}
