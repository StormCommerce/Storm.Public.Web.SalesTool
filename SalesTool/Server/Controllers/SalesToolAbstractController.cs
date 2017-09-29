using Enferno.Public.Logging;
using Enferno.StormApiClient;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Http;

namespace Enferno.Public.Web.SalesTool.Server.Controllers
{
    public abstract class SalesToolAbstractController : ApiController
    {
        protected IAccessClient Client;

        public SalesToolAbstractController()
        {
            Client = new AccessClient();
        }

        public void LogError(Exception ex, params object[] values)
        {
            var callingMethod = new StackTrace().GetFrame(1).GetMethod();
            string methodName = callingMethod.Name;
            var parameters = callingMethod.GetParameters();

            var entry = Log.LogEntry.Categories(CategoryFlags.Alert)
                .Message("Error in {0}.{1}: {2}", "SalesTool", methodName, ex.Message).Exceptions(ex);

            DumpParameterValues(entry, parameters, values);
            entry.WriteError();
        }

        private static void DumpParameterValues(LogEntryWrapper entry, ParameterInfo[] parameters, object[] values)
        {
            try
            {
                if (parameters == null || parameters.Length == 0 || values == null || values.Length == 0) return;
                for (int i = 0; i < parameters.Length; i++)
                    entry.Property(parameters[i].Name, values[i]);
            }
            catch (Exception ex)
            {
                entry.Exceptions(ex);
            }
        }
    }
}
