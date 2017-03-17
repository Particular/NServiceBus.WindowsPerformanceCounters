using System;
using System.Collections.Generic;
using NServiceBus;
using NServiceBus.Transport;

public static class PipelineHelper
{
    public static ReceivePipelineCompleted BuildPipelineCompleted(Dictionary<string, string> headers, DateTime startedAt, DateTime completedAt)
    {
        var message = new IncomingMessage("id", headers, new byte[]
        {
        });
        return new ReceivePipelineCompleted(message, startedAt, completedAt);
    }

}