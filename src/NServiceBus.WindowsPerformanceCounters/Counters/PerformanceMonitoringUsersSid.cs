namespace NServiceBus
{
    using System.Security.Principal;

    static class PerformanceMonitoringUsersSid
    {
        public static string Get()
        {
            var usersName = new SecurityIdentifier(WellKnownSidType.BuiltinPerformanceMonitoringUsersSid, null)
                .Translate(typeof(NTAccount))
                .ToString();
            var parts = usersName.Split('\\');

            if (parts.Length == 2)
            {
                return parts[1];
            }
            return usersName;
        }
    }
}