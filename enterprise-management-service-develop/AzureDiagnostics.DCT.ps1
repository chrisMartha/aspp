Param([string]$azureImport="C:\Program Files (x86)\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1",
      [string]$publishSettings=".\CCSoC-DCT-credentials.publishsettings",
      [string]$storageAccount="pemsdct",
	  [string]$storageAccountPrimaryAccessKey="JIXEgMpjCrHrNq2iyYqOlMycYemCqxBr2jkvRRP69CHNrw+Z+H1yBiXjC+aNc3b269hAiHVeKmx8sgKMpGraDg==",
      [string]$subscription="CCSoC DCT",
      [string]$serviceName="pems-dct",
      [string]$slot="Production",
      [string]$webRole = "PSoC.ManagementService",
      [string]$workerRole = "PSoC.ManagementService.LicenseTimer",
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

try {
    Write-Host "Adjusting configuration for local host environments..."
    if ($env:computername -eq "DEV-WIN01")
    {
        Write-Host "Recognized Roman's VM workstation DEV-WIN01 - compensating..."
		$wadConfigPath="C:\Projects\Pearson\enterprise-management-service\diagnostics.DCT.wadcfgx"
    }
    elseif ($env:computername -eq "CTSTX35285")
    {
        Write-Host "Recognized Aurele's VM workstation CTSTX35285 - compensating..."
		$wadConfigPath="C:\Aurele\01.Pearson\01.Projects\01.PEMs\05.SourceCode\diagnostics.DCT.wadcfgx"
		$azureImport="C:\Program Files\Microsoft SDKs\Azure\PowerShell\ServiceManagement\Azure\Azure.psd1"
    }

    Write-Host "Running Azure imports from $azureImport..."
    Import-Module $azureImport

    Write-Host "Gathering information..."
    if (!$subscription){    $subscription = Read-Host "Subscription (case-sensitive)"}
    if (!$storageAccount){  $storageAccount = Read-Host "Storage account name"}
    if (!$serviceName){     $serviceName = Read-Host "Cloud service name"}
    if (!$publishSettings){ $publishSettings = Get-File "Azure publish settings (*.publishsettings)|*.publishsettings"}

    Set-AzureSettings -publishsettings $publishSettings -subscription $subscription -storageaccount $storageAccount

	Write-Host "Creating Azure Context in storage account $storageAccount for diagnostics..."
	$storageContext = New-AzureStorageContext -StorageAccountName $storageAccount -StorageAccountKey $storageAccountPrimaryAccessKey
	Write-Host "Created Azure Context in storage account $storageAccount for diagnostics."
    Write-Host ""
	
	Write-Host "Removing cloud service diagnostics extension on $role1 role at $slot deployment slot..."
    Remove-AzureServiceDiagnosticsExtension -ServiceName $serviceName -Slot $slot -Role $webRole
	Write-Host "Removed cloud service diagnostics extension on $role1 role at $slot deployment slot."
    Write-Host ""

	Write-Host "Removing cloud service diagnostics extension on $role2 role at $slot deployment slot..."
    Remove-AzureServiceDiagnosticsExtension -ServiceName $serviceName -Slot $slot -Role $workerRole
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