using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceivePerformanceDiagnosticsBehavior : IBehavior<IIncomingPhysicalMessageContext, IIncomingPhysicalMessageContext>
{
    public ReceivePerformanceDiagnosticsBehavior(string queueName)
    {
        this.queueName = queueName;
    }

    public const string MessagesPulledPerSecondCounterName = "# of msgs pulled from the input queue /sec";
    public const string MessagesProcessedPerSecondCounterName = "# of msgs successfully processed / sec";
    public const string MessagesFailuresPerSecondCounterName = "# of msgs failures / sec";
    public void Warmup()
    {
        messagesPulledFromQueueCounter = PerformanceCounterHelper.InstantiatePerformanceCounter(
            MessagesPulledPerSecondCounterName,
            queueName);
        successRateCounter = PerformanceCounterHelper.InstantiatePerformanceCounter(
            MessagesProcessedPerSecondCounterName,
            queueName);
        failureRateCounter = PerformanceCounterHelper.InstantiatePerformanceCounter(
            MessagesFailuresPerSecondCounterName,
            queueName);
    }

    public async Task Invoke(IIncomingPhysicalMessageContext context, Func<IIncomingPhysicalMessageContext, Task> next)
    {
        messagesPulledFromQueueCounter.Increment();

        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (Exception)
        {
            failureRateCounter.Increment();
            throw;
        }

        successRateCounter.Increment();
    }

    public void Cooldown()
    {
        messagesPulledFromQueueCounter?.Dispose();
        successRateCounter?.Dispose();
        failureRateCounter?.Dispose();
    }

    IPerformanceCounterInstance failureRateCounter;
    IPerformanceCounterInstance messagesPulledFromQueueCounter;
    IPerformanceCounterInstance successRateCounter;

    string queueName;
}