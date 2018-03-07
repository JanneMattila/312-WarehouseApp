# To create securestring you can use following command:
# $password = ConvertTo-SecureString -String "password" -AsPlainText -Force
#
# .\deploy.ps1 -DatabaseAdminPassword $password
#
Param (
    [string] $ResourceGroupName = "warehouseapp-local-rg",
    [string] $Location = "West Europe",
    [string] $Template = "$PSScriptRoot\azuredeploy.json",
    [string] $TemplateParameters = "$PSScriptRoot\azuredeploy.parameters.json",
    [string] $PricingTier = "B1",
    [string] $DatabaseName = "WarehouseDatabase",
    [string] $DatabaseAdminUser = "sqluser",
    [Parameter(Mandatory=$true)] [securestring] $DatabaseAdminPassword
)

$ErrorActionPreference = "Stop"

$date = (Get-Date).ToString("yyyy-MM-dd-HH-mm-ss")
$deploymentName = "Local-$date"

if ([string]::IsNullOrEmpty($env:RELEASE_DEFINITIONNAME))
{
    Write-Host (@"
Not executing inside VSTS Release Management.
Make sure you have done "Login-AzureRmAccount" and
"Select-AzureRmSubscription -SubscriptionName name"
so that script continues to work correctly for you.
"@)
}
else
{
	$deploymentName = $env:RELEASE_RELEASENAME
}

if ((Get-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -ErrorAction SilentlyContinue) -eq $null)
{
    Write-Warning "Resource group '$ResourceGroupName' doesn't exist and it will be created."
    New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -Verbose
}

# Create additional parameters that we pass to the template deployment
$additionalParameters = New-Object -TypeName hashtable
$additionalParameters['warehouseAppPlanSkuName'] = $PricingTier
$additionalParameters['databaseName'] = $DatabaseName
$additionalParameters['administratorLogin'] = $DatabaseAdminUser
$additionalParameters['administratorLoginPassword'] = $DatabaseAdminPassword

$result = New-AzureRmResourceGroupDeployment `
	-DeploymentName $deploymentName `
    -ResourceGroupName $ResourceGroupName `
    -TemplateFile $Template `
    -TemplateParameterFile $TemplateParameters `
    @additionalParameters `
	-Verbose

$result

if ($result.Outputs.webAppName -eq $null -or
    $result.Outputs.webAppUri -eq $null -or
    $result.Outputs.sqlServerName -eq $null -or
    $result.Outputs.sqlServerConnectionString -eq $null)
{
    Throw "Template deployment didn't return 'output' variables correctly and therefore deployment is cancelled."
}

$webAppName = $result.Outputs.webAppName.value
$webAppUri = $result.Outputs.webAppUri.value
$sqlServerName = $result.Outputs.sqlServerName.value
$sqlServerConnectionString = $result.Outputs.sqlServerConnectionString.value

Write-Host "##vso[task.setvariable variable=Custom.WebAppName;]$webAppName"
Write-Host "##vso[task.setvariable variable=Custom.WebAppUri;]$webAppUri"
Write-Host "##vso[task.setvariable variable=Custom.SqlServerName;]$sqlServerName"
Write-Host "##vso[task.setvariable variable=Custom.DatabaseName;]$DatabaseName"

Write-Host "Validating that site is up and running..."
$running = 0
for ($i = 0; $i -lt 60; $i++)
{
    try 
    {
        $request = Invoke-WebRequest -Uri $webAppUri -UseBasicParsing -DisableKeepAlive -ErrorAction SilentlyContinue
        Write-Host "Site status code $($request.StatusCode)."

        if ($request.StatusCode -eq 200)
        {
            Write-Host "Site is up and running."
            $running++
        }
    }
    catch
    {
        Start-Sleep -Seconds 3
    }

    if ($running -eq 10)
    {
        return
    }
}

Throw "Site didn't respond on defined time."
