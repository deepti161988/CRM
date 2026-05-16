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
    public class EmployeeCreateValidateEmail : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get services
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                factory.CreateOrganizationService(context.UserId);
            try
            {
                // Run only for Create
                if (context.MessageName != "Create")
                    return;

                // Check Target
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                    return;

                Entity entity = (Entity)context.InputParameters["Target"];

                // Ensure correct entity
                if (entity.LogicalName != "abc_employee")
                    return;

                tracingService.Trace("Pre-Validation Plugin Triggered - Duplicate Email Check");

                // 🔹 Check Email exists in target
                if (!entity.Contains("abc_email") || string.IsNullOrWhiteSpace(entity["abc_email"].ToString()))
                {
                    throw new InvalidPluginExecutionException("Email cannot be empty.");
                }

                string email = entity["abc_email"].ToString();

                // 🔹 Query to check duplicate email
                QueryExpression query = new QueryExpression("abc_employee");
                query.ColumnSet = new ColumnSet("abc_email");

                query.Criteria.AddCondition("abc_email", ConditionOperator.Equal, email);

                EntityCollection result = service.RetrieveMultiple(query);

                // 🔹 If duplicate found → stop execution
                if (result.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException(
                        "Employee with this email already exists. Please use a different email.");
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
