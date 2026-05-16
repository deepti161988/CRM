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
    public class EmployeePreDelete : IPlugin
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
            try
            {
                // Run only on Delete
                if (context.MessageName != "Delete")
                    return;

                // Check Target
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is EntityReference))
                    return;

                EntityReference targetRef =
                    (EntityReference)context.InputParameters["Target"];

                // Ensure correct entity
                if (targetRef.LogicalName != "abc_employee")
                    return;

                tracingService.Trace("Pre-Delete Plugin Triggered");

                // 🔹 Retrieve employee record
                Entity employee = service.Retrieve(
                    "abc_employee",
                    targetRef.Id,
                    new ColumnSet("abc_employeename", "abc_joiningdate")
                );

                // 🔹 Create backup record
                Entity backup = new Entity("abc_employeebackup");

                // Employee Name
                if (employee.Contains("abc_employeename"))
                    backup["abc_employeename"] = employee["abc_employeename"];

                // Joining Date → Backup field
                if (employee.Contains("abc_joiningdate"))
                    backup["abc_joiningdate_backup"] = employee["abc_joiningdate"];

                // Deleted On → Current Date
                backup["abc_deletedon"] = DateTime.UtcNow;

                // 🔹 Create backup record
                service.Create(backup);

                tracingService.Trace("Backup record created successfully");
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error: " + ex.ToString());
                throw new InvalidPluginExecutionException("Error in Pre-Delete Backup Plugin: " + ex.Message);
            }
        }
    }
}
