using System;
using NServiceBus;

static class PipelineExtensions
{
    public static bool TryGetTimeSent(this ReceivePipelineCompleted completed, out DateTime timeSent)
    {
        var headers = completed.ProcessedMessage.Headers;
        string timeSentString;
        if (headers.TryGetValue(Headers.TimeSent, out timeSentString))
        {
            timeSent = DateTimeExtensions.ToUtcDateTime(timeSentString);
            return true;
        }
        timeSent = DateTime.MinValue;
        return false;
    }
}
