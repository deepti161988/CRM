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
    public class EmployeeExperienceBasedOnDOJ : IPlugin
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
            try
            {
                // Run only for Create or Update
                if (context.MessageName != "Create" && context.MessageName != "Update")
                    return;

                // Check Target
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                    return;

                Entity entity = (Entity)context.InputParameters["Target"];

                // Ensure correct entity
                if (entity.LogicalName != "abc_employee")
                    return;

                tracingService.Trace("Pre-Operation Plugin Triggered - Experience Calculation");

                // Check Joining Date exists
                if (entity.Contains("abc_joiningdate") && entity["abc_joiningdate"] != null)
                {
                    DateTime joiningDate = (DateTime)entity["abc_joiningdate"];

                    // Calculate experience in years
                    int experience = DateTime.UtcNow.Year - joiningDate.Year;

                    // Adjust if current date is before joining anniversary
                    if (DateTime.UtcNow < joiningDate.AddYears(experience))
                    {
                        experience--;
                    }

                    tracingService.Trace("Calculated Experience: " + experience);

                    // Set experience field
                    entity["abc_experience"] = experience;
                }
                else
                {
                    throw new InvalidPluginExecutionException("Joining Date is required to calculate experience.");
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("Error in Experience Calculation: " + ex.Message);
            }
        }
    }

  
}
