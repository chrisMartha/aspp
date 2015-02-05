Param([string]$azureImport="C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1",
      [string]$publishSettings=".\CCSoC-DCT-credentials.publishsettings",
      [string]$storageAccount="pemsdct",
	  [string]$storageAccountPrimaryAccessKey="JIXEgMpjCrHrNq2iyYqOlMycYemCqxBr2jkvRRP69CHNrw+Z+H1yBiXjC+aNc3b269hAiHVeKmx8sgKMpGraDg==",
      [string]$subscription="CCSoC DCT",
      [string]$serviceName="pems-dct",
      [string]$containerName="mydeployments",
      [string]$config=".\AzureDeploy\bin\DCT\app.publish\ServiceConfiguration.DCT.cscfg",
      [string]$package=".\AzureDeploy\bin\DCT\app.publish\AzureDeploy.cspkg",
      [string]$slot="Production",
      [string]$role1 = "PSoC.ManagementService",
      [string]$role2 = "PSoC.ManagementService.LicenseTimer",
	  [string]$wadConfigPath=".\diagnostics.DCT.wadcfgx")

Function Get-File($filter) {
    [System.Reflection.Assembly]::LoadWithPartialName("System.windows.forms") | Out-Null
    $fd = New-Object system.windows.forms.openfiledialog
    $fd.MultiSelect = $false
    $fd.Filter = $filter
    [void]$fd.showdialog()
    return $fd.FileName
}

Function Set-AzureSettings($publishSettings, $subscription, $storageAccount) {
    Write-Host "Importing publish setting file from $publishSettings..."
    Import-AzurePublishSettingsFile $publishSettings

    Write-Host "Setting Azure subscription to $subscription with storage account $storageAccount..."
    Set-AzureSubscription -SubscriptionName $subscription -CurrentStorageAccount $storageAccount

    Write-Host "Selecting Azure subscription $subscription..."
    Select-AzureSubscription $subscription
}

Function Upload-Package($package, $containerName) {
    $blob = "$serviceName.package.$(get-date -f yyyy_MM_dd_hh_ss).cspkg"
    
    $containerState = Get-AzureStorageContainer -Name $containerName -ea 0
    if ($containerState -eq $null) {
        New-AzureStorageContainer -Name $containerName | out-null
    }
    
    Set-AzureStorageBlobContent -File $package -Container $containerName -Blob $blob -Force| Out-Null
    $blobState = Get-AzureStorageBlob -blob $blob -Container $containerName

    $blobState.ICloudBlob.uri.AbsoluteUri
}

Function Create-Deployment($package_url, $serviceName, $slot, $config) {
    $opstat = New-AzureDeployment -Slot $slot -Package $package_url -Configuration $config -ServiceName $serviceName
}
 
Function Upgrade-Deployment($package_url, $serviceName, $slot, $config) {
	$label = "Manual AzureDeploy $(Get-Date -Format yyyy-MM-dd) $(Get-Date -Format HH:ss) ET"
    $setdeployment = Set-AzureDeployment -Upgrade -Slot $slot -Package $package_url -Configuration $config -ServiceName $serviceName -Force -Label $label
}

Function Check-Deployment($serviceName, $slot) {
    $completeDeployment = Get-AzureDeployment -ServiceName $serviceName -Slot $slot
    $completeDeployment.deploymentid
}

try {
    Write-Host "Adjusting configuration for local host environments..."
    if ($env:computername -eq "DEV-WIN01")
    {
        Write-Host "Recognized Roman's VM workstation DEV-WIN01 - compensating..."
        $config = "C:\Projects\Pearson\enterprise-management-service\AzureDeploy\bin\DCT\app.publish\ServiceConfiguration.DCT.cscfg"
		$wadConfigPath="C:\Projects\Pearson\enterprise-management-service\diagnostics.DCT.wadcfgx"
    }

    Write-Host "Running Azure imports from $azureImport..."
    Import-Module $azureImport

    Write-Host "Gathering information..."
    if (!$subscription){    $subscription = Read-Host "Subscription (case-sensitive)"}
    if (!$storageAccount){  $storageAccount = Read-Host "Storage account name"}
    if (!$serviceName){     $serviceName = Read-Host "Cloud service name"}
    if (!$publishSettings){ $publishSettings = Get-File "Azure publish settings (*.publishsettings)|*.publishsettings"}
    if (!$package){         $package = Get-File "Azure package (*.cspkg)|*.cspkg"}
    if (!$config){          $config = Get-File "Azure config file (*.cspkg)|*.cscfg"}

    Set-AzureSettings -publishsettings $publishSettings -subscription $subscription -storageaccount $storageAccount

    Write-Host "Uploading the deployment package $package with container name $containerName..."
    $package_url = Upload-Package -package $package -containerName $containerName
    Write-Host "Package uploaded to $package_url."

	Write-Host "Uploading package..."
    $deployment = Get-AzureDeployment -ServiceName $serviceName -Slot $slot -ErrorAction silentlycontinue 

    if ($deployment.Name -eq $null) {
        Write-Host "No deployment is detected. Creating a new deployment in slot $slot..."
        Create-Deployment -package_url $package_url -service $serviceName -slot $slot -config $config
        Write-Host "New deployment created."
    } else {
        Write-Host "Deployment exists in $serviceName.  Upgrading deployment in slot $slot..."
        Upgrade-Deployment -package_url $package_url -service $serviceName -slot $slot -config $config
        Write-Host "Upgraded deployment."
    }

    $deploymentId = Check-Deployment -service $serviceName -slot $slot
    Write-Host "Successfully deployed to $serviceName with deployment id $deploymentId!"
    Write-Host ""
	
	Write-Host "Creating Azure Context in storage account $storageAccount for diagnostics..."
	$storageContext = New-AzureStorageContext -StorageAccountName $storageAccount -StorageAccountKey $storageAccountPrimaryAccessKey
	Write-Host "Created Azure Context in storage account $storageAccount for diagnostics."
    Write-Host ""
	
	Write-Host "Removing cloud service diagnostics extension on $role1 role at $slot deployment slot..."
    Remove-AzureServiceDiagnosticsExtension -ServiceName $serviceName -Slot $slot -Role $role1
	Write-Host "Removed cloud service diagnostics extension on $role1 role at $slot deployment slot."
    Write-Host ""

	Write-Host "Removing cloud service diagnostics extension on $role2 role at $slot deployment slot..."
    Remove-AzureServiceDiagnosticsExtension -ServiceName $serviceName -Slot $slot -Role $role2
	Write-Host "Removed cloud service diagnostics extension on $role2 role at $slot deployment slot."
    Write-Host ""

	Write-Host "Enabling cloud service diagnostics extension on all roles at $slot deployment slot..."
    Set-AzureServiceDiagnosticsExtension -StorageContext $storageContext -DiagnosticsConfigurationPath $wadConfigPath -ServiceName $serviceName -Slot $slot
	Write-Host "Enabled cloud service diagnostics extension on all roles at $slot deployment slot."
    Write-Host ""

	Write-Host "Getting the cloud service diagnostics extension applied on roles at $slot deployment slot..."
    Get-AzureServiceDiagnosticsExtension -ServiceName $serviceName -Slot $slot

    exit 0
}
catch [System.Exception] {
    Write-Host $_.Exception.ToString()
    exit 1
}