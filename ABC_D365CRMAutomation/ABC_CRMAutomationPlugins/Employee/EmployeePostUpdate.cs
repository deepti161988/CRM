using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace ABC_CRMAutomationPlugins.Employee
{
    public class EmployeePostUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
           
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            //Message Name
            tracingService.Trace("Operation Name: {0}", context.MessageName);

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "abc_employee")
                    return;

                tracingService.Trace("abc_employee Name: {0}", entity["abc_employeename"]);
            }

            //preimage
            if (context.PreEntityImages.Contains("PreImage"))
            {
                Entity preimage = context.PreEntityImages["PreImage"];
                if (preimage.Contains("abc_employeename"))
                {
                    tracingService.Trace("pre abc_employeename: {0}", preimage["abc_employeename"]);
                }
            }

            //post image
            if (context.PostEntityImages.Contains("PostImage"))
            {
                Entity postimage = context.PostEntityImages["PostImage"];
                if (postimage.Contains("abc_employeename"))
                {
                    tracingService.Trace("post abc_employeename: {0}", postimage["abc_employeename"]);
                }
            }


        }
        public void Execute_old(IServiceProvider serviceProvider)

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
                if (context.MessageName != "Update")
                    return;

                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                    return;

                Entity entity = (Entity)context.InputParameters["Target"];

                if (entity.LogicalName != "abc_employee")
                    return;

                // 🔥 Prevent infinite loop
                if (context.Depth > 1)
                    return;

                // Check if statecode is being updated
                if (entity.Contains("statecode"))
                {
                    OptionSetValue state = (OptionSetValue)entity["statecode"];

                    tracingService.Trace("StateCode: {0}", state.Value);

                    // Prepare update entity
                    Entity updateEntity = new Entity(entity.LogicalName);
                    updateEntity.Id = entity.Id;

                    if (state.Value == 1) // Inactive
                    {
                        tracingService.Trace("Setting termination date");

                        updateEntity["abc_terminationdate"] = DateTime.UtcNow;
                    }
                    else if (state.Value == 0) // Active
                    {
                        tracingService.Trace("Clearing termination date");

                        updateEntity["abc_terminationdate"] = null;
                    }

                    service.Update(updateEntity);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error: " + ex.Message);
            }

        }
    }
}
