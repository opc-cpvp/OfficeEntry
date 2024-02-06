using Microsoft.Xrm.Sdk;
using System;

namespace OfficeEntry.Plugins
{
    /*
     * Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
     * Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
     */
    public class AccessRequestPlugin : PluginBase
    {
        public AccessRequestPlugin(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(AccessRequestPlugin))
        {
        }

        public AccessRequestPlugin() : base(typeof(AccessRequestPlugin)) { }

        // Entry point for custom business logic execution
        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
                throw new ArgumentNullException("localPluginContext");

            var context = localPluginContext.PluginExecutionContext;

            if (context.MessageName != PluginMessage.Create && context.MessageName != PluginMessage.Update)
                return;

            if (!(context.InputParameters["Target"] is Entity target))
                return;

            try
            {
                // Convert the target entity to an AccessRequest
                var accessRequest = target.ToEntity<gc_accessrequest>();

                // Assign the AccessRequest's ID to the Guid property
                accessRequest.gc_guid = accessRequest.gc_accessrequestId?.ToString("D");

                


                var dayValue = accessRequest.gc_starttime;

                switch (dayValue.Value.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Monday); 
                        break;
                    case DayOfWeek.Tuesday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Tuesday);
                        break;
                    case DayOfWeek.Wednesday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Wednesday);
                        break;
                    case DayOfWeek.Thursday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Thursday);
                        break;
                    case DayOfWeek.Friday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Friday);
                        break;
                    case DayOfWeek.Saturday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Saturday);
                        break;
                    case DayOfWeek.Sunday:
                        accessRequest.gc_dayofweek = new OptionSetValue((int)gc_accessrequest_gc_dayofweek.Sunday);
                        break;
                }

                localPluginContext.CurrentUserService.Update(accessRequest);


            }
            catch (Exception ex)
            {
                // Trace and throw any exceptions
                localPluginContext.TracingService.Trace($"Exception: {ex.Message} - Stack Trace: {ex.StackTrace}");
                throw new InvalidPluginExecutionException($"An error occurred in the plug-in. AccessRequest: {ex.Message}", ex);
            }

        }
    }
}