#requires -RunAsAdministrator
Function InstallNSBPerfCounters {
    
    $category = @{Name="NServiceBus"; Description="NServiceBus statistics"}
    $counters = New-Object System.Diagnostics.CounterCreationDataCollection
    $counters.AddRange(@(
        New-Object System.Diagnostics.CounterCreationData "SLA violation countdown", "Seconds until the SLA for this endpoint is breached.",  NumberOfItems32
        New-Object System.Diagnostics.CounterCreationData "Critical Time", "The time it took from sending to processing the message.",  NumberOfItems32
        New-Object System.Diagnostics.CounterCreationData "Processing Time", "The time it took to successfully process a message.",  NumberOfItems32
        New-Object System.Diagnostics.CounterCreationData "# of msgs pulled from the input queue /sec", "The current number of messages pulled from the input queue by the transport per second.",  RateOfCountsPerSecond32
        New-Object System.Diagnostics.CounterCreationData "# of msgs failures / sec", "The current number of failed processed messages by the transport per second.",  RateOfCountsPerSecond32
        New-Object System.Diagnostics.CounterCreationData "# of msgs successfully processed / sec", "The current number of messages processed successfully by the transport per second.",  RateOfCountsPerSecond32
    ))

    if ([System.Diagnostics.PerformanceCounterCategory]::Exists($category.Name)) {

       foreach($counter in $counters){
            $exists = [System.Diagnostics.PerformanceCounterCategory]::CounterExists($counter.CounterName, $category.Name)
            if (!$exists){
                Write-Host "One or more counters are missing.The performance counter category will be recreated"
                [System.Diagnostics.PerformanceCounterCategory]::Delete($category.Name)

                break
            }
        }
    }

    if (![System.Diagnostics.PerformanceCounterCategory]::Exists($category.Name)) {
        Write-Host "Creating the performance counter category"
        [void] [System.Diagnostics.PerformanceCounterCategory]::Create($category.Name, $category.Description, [System.Diagnostics.PerformanceCounterCategoryType]::MultiInstance, $counters)
        }
    else {
        Write-Host "No performance counters have to be created"
    }

    [System.Diagnostics.PerformanceCounter]::CloseSharedResources()
}
InstallNSBPerfCounters