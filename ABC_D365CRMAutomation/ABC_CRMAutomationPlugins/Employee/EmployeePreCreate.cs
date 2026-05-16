using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SharedMetadata;


namespace ABC_CRMAutomationPlugins.Employee
{
    public class EmployeePreCreate : IPlugin
    {

        private string _unsecureConfig;
        private string _secureConfig;

        // Constructor to read configs
        public EmployeePreCreate(string unsecureConfig, string secureConfig)
        {
            _unsecureConfig = unsecureConfig;
            _secureConfig = secureConfig;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new InvalidPluginExecutionException("Deepti, here wrong in the plugin.");
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            //Message Name
            tracingService.Trace("Operation Name: {0}", context.MessageName);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                //Entity entity = (Entity)context.InputParameters["Target"];
                abc_Employee abcemployee = (abc_Employee)context.InputParameters["Target"];


                if (abcemployee.LogicalName != abc_Employee.EntityLogicalName )
                    return;

                tracingService.Trace("Setting Gender to Male");

                // ✅ Force gender = true (Male)
                //entity["abc_gender"] = true;
                abcemployee.abc_Gender = true;
                
                abcemployee.abc_Hobby = abc_employee_abc_hobby.Cricket;

                //entity["abc_address"] = "pune1 as initial address";

                //// Example usage
                //if (!string.IsNullOrEmpty(_unsecureConfig))
                //{
                //    // Do something with unsecure config
                //    entity["abc_address"] = _unsecureConfig;
                //}

                //if (!string.IsNullOrEmpty(_secureConfig))
                //{

                //    // Do something with secure config
                //    entity["abc_address"] = _secureConfig;
                //}

                string address = GetEnvironmentVariableValue1(service, "abc_EmployeeInitialAddress");

                if (!string.IsNullOrEmpty(address))
                {
                    //entity["abc_address"] = address;
                    abcemployee.abc_Address = address;
                }

            }
        }


        public static string GetEnvironmentVariableValue1(IOrganizationService service, string schemaName)
        {
            var query = new QueryExpression("environmentvariabledefinition")
            {
                ColumnSet = new ColumnSet("schemaname", "defaultvalue"),
                Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("schemaname", ConditionOperator.Equal, schemaName)
                            }
                        },
                LinkEntities =
                {
                    new LinkEntity
                    {
                        LinkFromEntityName = "environmentvariabledefinition",
                        LinkFromAttributeName = "environmentvariabledefinitionid",
                        LinkToEntityName = "environmentvariablevalue",
                        LinkToAttributeName = "environmentvariabledefinitionid",
                        JoinOperator = JoinOperator.LeftOuter,
                        Columns = new ColumnSet("value"),
                        EntityAlias = "envValue"
                    }
                }
                    };

            var result = service.RetrieveMultiple(query).Entities.FirstOrDefault();

            if (result == null)
                return null;

            // Get current value
            if (result.Contains("envValue.value"))
            {
                var aliasedValue = (AliasedValue)result["envValue.value"];
                if (aliasedValue?.Value != null)
                    return aliasedValue.Value.ToString();
            }

            // Fallback to default value
            return result.GetAttributeValue<string>("defaultvalue");
        }
    }
}


