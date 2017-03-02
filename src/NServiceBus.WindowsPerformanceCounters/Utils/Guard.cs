namespace NServiceBus
{
    using JetBrains.Annotations;
    using System;

    static class Guard
    {
        [ContractAnnotation("value: null => halt")]
        public static void AgainstNull([InvokerParameterName] string argumentName, [NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void AgainstNegativeAndZero([InvokerParameterName] string argumentName, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}
