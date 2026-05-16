using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace ABC_CRMAutomationPlugins.Employee
{
   
    public class EmployeePreValidate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get services
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                // Run only for Create/Update
                if (context.MessageName != "Create" && context.MessageName != "Update")
                    return;

                // Check Target
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                    return;

                Entity entity = (Entity)context.InputParameters["Target"];

                // Ensure correct entity
                if (entity.LogicalName != "abc_employee")
                    return;

                tracingService.Trace("Pre-Validation Plugin Triggered");

                // 🔹 Validate Employee Name
                if (entity.Contains("abc_employeename"))
                {
                    string name = entity["abc_employeename"].ToString();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new InvalidPluginExecutionException("Employee Name cannot be empty.");
                    }
                }

                // 🔹 Validate Joining Date
                if (entity.Contains("abc_joiningdate"))
                {
                    DateTime joiningDate = (DateTime)entity["abc_joiningdate"];

                    if (joiningDate > DateTime.UtcNow)
                    {
                        throw new InvalidPluginExecutionException("Joining date cannot be in the future.");
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Validation Error: " + ex.Message);
            }
        }
    }
}
