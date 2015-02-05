Param([string]$azureImport="C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1",
      [string]$publishSettings=".\CCSoC-Development-credentials.publishsettings",
      [string]$storageAccount="pemsqa",
      [string]$subscription="CCSoC Development",
      [string]$service="pems-qa",
      [string]$containerName="mydeployments",
      [string]$config=".\AzureDeploy\bin\QA\app.publish\ServiceConfiguration.QA.cscfg",
      [string]$package=".\AzureDeploy\bin\QA\app.publish\AzureDeploy.cspkg",
      [string]$slot="Production")

Function Get-File($filter){
    [System.Reflection.Assembly]::LoadWithPartialName("System.windows.forms") | Out-Null
    $fd = New-Object system.windows.forms.openfiledialog
    $fd.MultiSelect = $false
    $fd.Filter = $filter
    [void]$fd.showdialog()
    return $fd.FileName
}

Function Set-AzureSettings($publishSettings, $subscription, $storageAccount){
    Write-Host "Importing publish setting file from $publishSettings..."
    Import-AzurePublishSettingsFile $publishSettings

    Write-Host "Setting Azure subscription to $subscription with storage account $storageAccount..."
    Set-AzureSubscription -SubscriptionName $subscription -CurrentStorageAccount $storageAccount

    Write-Host "Selecting Azure subscription $subscription..."
    Select-AzureSubscription $subscription
}

Function Upload-Package($package, $containerName){
    $blob = "$service.package.$(get-date -f yyyy_MM_dd_hh_ss).cspkg"
    
    $containerState = Get-AzureStorageContainer -Name $containerName -ea 0
    if ($containerState -eq $null)
    {
        New-AzureStorageContainer -Name $containerName | out-null
    }
    
    Set-AzureStorageBlobContent -File $package -Container $containerName -Blob $blob -Force| Out-Null
    $blobState = Get-AzureStorageBlob -blob $blob -Container $containerName

    $blobState.ICloudBlob.uri.AbsoluteUri
}

Function Create-Deployment($package_url, $service, $slot, $config){
    $opstat = New-AzureDeployment -Slot $slot -Package $package_url -Configuration $config -ServiceName $service
}
 
Function Upgrade-Deployment($package_url, $service, $slot, $config){
	$label = "Jenkins AzureDeploy $(Get-Date -Format yyyy-MM-dd) $(Get-Date -Format HH:ss) ET"
    $setdeployment = Set-AzureDeployment -Upgrade -Slot $slot -Package $package_url -Configuration $config -ServiceName $service -Force -Label $label
}

Function Check-Deployment($service, $slot){
    $completeDeployment = Get-AzureDeployment -ServiceName $service -Slot $slot
    $completeDeployment.deploymentid
}

try {
    Write-Host "Adjusting configuration for local host environments..."
    if ($env:computername -eq "DEV-WIN01")
    {
        Write-Host "Recognized Roman's VM workstation DEV-WIN01 - compensating..."
        $config = "C:\Projects\Pearson\enterprise-management-service\AzureDeploy\bin\QA\app.publish\ServiceConfiguration.QA.cscfg"
    }

    Write-Host "Running Azure imports from $azureImport..."
    Import-Module $azureImport

    Write-Host "Gathering information..."
    if (!$subscription){    $subscription = Read-Host "Subscription (case-sensitive)"}
    if (!$storageAccount){  $storageAccount = Read-Host "Storage account name"}
    if (!$service){         $service = Read-Host "Cloud service name"}
    if (!$publishSettings){ $publishSettings = Get-File "Azure publish settings (*.publishsettings)|*.publishsettings"}
    if (!$package){         $package = Get-File "Azure package (*.cspkg)|*.cspkg"}
    if (!$config){          $config = Get-File "Azure config file (*.cspkg)|*.cscfg"}

    Set-AzureSettings -publishsettings $publishSettings -subscription $subscription -storageaccount $storageAccount

    Write-Host "Uploading the deployment package $package with container name $containerName..."
    $package_url = Upload-Package -package $package -containerName $containerName
    Write-Host "Package uploaded to $package_url."

	Write-Host "Uploading package..."
    $deployment = Get-AzureDeployment -ServiceName $service -Slot $slot -ErrorAction silentlycontinue 

    if ($deployment.Name -eq $null) {
        Write-Host "No deployment is detected. Creating a new deployment in slot $slot..."
        Create-Deployment -package_url $package_url -service $service -slot $slot -config $config
        Write-Host "New deployment created."

    } else {
        Write-Host "Deployment exists in $service.  Upgrading deployment in slot $slot..."
        Upgrade-Deployment -package_url $package_url -service $service -slot $slot -config $config
        Write-Host "Upgraded deployment."
    }

    $deploymentid = Check-Deployment -service $service -slot $slot
    Write-Host "Successfully deployed to $service with deployment id $deploymentid!"
    exit 0
}
catch [System.Exception] {
    Write-Host $_.Exception.ToString()
    exit 1
}