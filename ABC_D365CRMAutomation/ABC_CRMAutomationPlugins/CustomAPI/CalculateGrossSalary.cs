using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ABC_CRMAutomationPlugins.CustomAPI
{
    public class CalculateGrossSalary : IPlugin
    {
  
            public void Execute(IServiceProvider serviceProvider)
            {
                ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                IPluginExecutionContext context =
                    (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                try
                {
                    tracingService.Trace("Gross Salary Plugin Started");

                    // 🔹 Read Inputs
                    decimal salary = 0;
                    bool isMetro = false;

                    if (context.InputParameters.Contains("abc_Salary"))
                    {
                        salary = Convert.ToDecimal(context.InputParameters["abc_Salary"]);
                    }

                    if (context.InputParameters.Contains("abc_IsMetro"))
                    {
                        isMetro = Convert.ToBoolean(context.InputParameters["abc_IsMetro"]);
                    }

                    tracingService.Trace("Salary: " + salary);
                    tracingService.Trace("IsMetro: " + isMetro);

                    // 🔹 Business Logic
                    decimal hra = isMetro ? salary * 0.30M : salary * 0.20M;
                    decimal bonus = salary * 0.10M;

                    decimal grossSalary = salary + hra + bonus;

                    tracingService.Trace("Gross Salary: " + grossSalary);

                    // 🔹 Output
                    context.OutputParameters["abc_GrossSalary"] = grossSalary;
                    context.OutputParameters["abc_Message"] =
                        $"Gross salary calculated: {grossSalary}";
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        
    }
}
