using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ABC_CRMAutomationPlugins.CustomAPI
{
    public class CalculateTotalWithTax : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get tracing service
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService.Trace("Plugin execution started.");

            // Get context
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("Context obtained successfully.");

            try
            {
                // 🔷 Check Input Parameters
                tracingService.Trace("Checking input parameters...");

                if (context.InputParameters == null || context.InputParameters.Count == 0)
                {
                    tracingService.Trace("No input parameters found.");
                    throw new InvalidPluginExecutionException("Input parameters are missing.");
                }   

                tracingService.Trace("Input parameters count: " + context.InputParameters.Count);

                tracingService.Trace("Listing all input parameters:");

                foreach (var param in context.InputParameters)
                {
                    tracingService.Trace("Key: " + param.Key + " | Value: " + (param.Value ?? "null"));
                }

                // 🔷 Read Name
                string name = string.Empty;

                if (context.InputParameters.Contains("abc_Name") && context.InputParameters["abc_Name"] != null)
                {
                    name = context.InputParameters["abc_Name"].ToString();
                    tracingService.Trace("Name received: " + name);
                }
                else
                {
                    tracingService.Trace("Name parameter missing or null.");
                }

                // 🔷 Read Amount
                decimal amount = 0;

                if (context.InputParameters.Contains("abc_Amount") && context.InputParameters["abc_Amount"] != null)
                {
                    amount = Convert.ToDecimal(context.InputParameters["abc_Amount"]);
                    tracingService.Trace("Amount received: " + amount);
                }
                else
                {
                    tracingService.Trace("Amount parameter missing or null.");
                }

                // 🔷 Business Logic
                tracingService.Trace("Applying business logic...");

                decimal taxRate = 0.18M;
                decimal totalAmount = amount + (amount * taxRate);

                tracingService.Trace("Calculated totalAmount: " + totalAmount);

                // 🔷 Prepare Output
                string resultMessage = $"Hello {name}, total amount with tax is {totalAmount}";
                tracingService.Trace("Result message created.");

                // 🔷 Set Output Parameters
                tracingService.Trace("Setting output parameters...");

                context.OutputParameters["abc_Result"] = resultMessage;
                context.OutputParameters["abc_TotalAmount"] = totalAmount;

                tracingService.Trace("Output parameters set successfully.");
                tracingService.Trace("Plugin execution completed successfully.");
            }
            catch (Exception ex)
            {
                tracingService.Trace("Exception occurred: " + ex.ToString());
                throw new InvalidPluginExecutionException("Error in Custom API: " + ex.Message);
            }
        }

    }
}
