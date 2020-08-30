using Microsoft.Xrm.Sdk;
using OfficeEntry.Plugins.Entities;
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

        // Entry point for custom business logic execution
        protected override void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
                throw new ArgumentNullException("localPluginContext");

            var context = localPluginContext.PluginExecutionContext;

            if (context.MessageName != PluginMessage.Create)
                return;

            if (!(context.InputParameters["Target"] is Entity target))
                return;

            try
            {
                // Convert the target entity to an AccessRequest
                var accessRequest = target.ToEntity<gc_accessrequest>();

                // Assign the AccessRequest's ID to the Guid property
                accessRequest.gc_guid = accessRequest.gc_accessrequestid?.ToString("D");

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