############################################################
# Common functions
############################################################

function Get-ScriptDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value;
    if($Invocation.PSScriptRoot)
    {
        $Invocation.PSScriptRoot;
    }
    Elseif($Invocation.MyCommand.Path)
    {
        Split-Path $Invocation.MyCommand.Path
    }
    else
    {
        $Invocation.InvocationName.Substring(0,$Invocation.InvocationName.LastIndexOf("\"));
    }
}


############################################################
# Check is IIS is installed
############################################################

$webserver = Get-Service W3SVC -ErrorAction SilentlyContinue
if( $webserver -eq $null ) {
    Write-Host "IIS is not installed. Script will not continue."
	Exit
}


# Remove existing web sites and AppPool
Write-Host 'Removing previously installed .NET Lab IIS Web sites...'
Remove-WebSite 'LabWebSite' -ErrorAction SilentlyContinue
Remove-WebSite 'ApiWebSite' -ErrorAction SilentlyContinue
Remove-WebSite 'HealthCheckWebSite' -ErrorAction SilentlyContinue
Remove-WebAppPool 'DotNetLabAppPool' -ErrorAction SilentlyContinue


# Install IIS AppPool to run as local system
Write-Host 'Installing .NET Lab AppPool...'

New-WebAppPool -Name 'DotNetLabAppPool'

$configSection = "/system.applicationHost/applicationPools/add[@name='DotNetLabAppPool']/processModel"
set-webconfigurationproperty $configSection -name identityType -value 0 -PSPath iis:\

$configSection = "/system.applicationHost/applicationPools/add[@name='DotNetLabAppPool']"
set-webconfigurationproperty $configSection -name managedRuntimeVersion -value 'v4.0' -PSPath iis:\


############################################################
# Install IIS web sites
############################################################

Write-Host 'Installing .NET Lab IIS Web Sites...'

$location = Get-ScriptDirectory

$labwebsitepath = Join-Path $location 'LabWebSite'
New-Website 'LabWebSite' -Port 2100 -ApplicationPool 'DotNetLabAppPool' -PhysicalPath $labwebsitepath

$apiwebsitepath = Join-Path $location 'ApiWebSite'
New-Website 'ApiWebSite' -Port 2101 -ApplicationPool 'DotNetLabAppPool' -PhysicalPath $apiwebsitepath

$healthchecksitepath = Join-Path $location 'HealthCheckWebSite'
New-Website 'HealthCheckWebSite' -Port 2102 -ApplicationPool 'DotNetLabAppPool' -PhysicalPath $healthchecksitepath

$ajaxpath = Join-Path $location 'Ajax'
New-WebApplication 'Ajax' -Site 'LabWebSite' -ApplicationPool 'DotNetLabAppPool' -PhysicalPath $ajaxpath


############################################################
# Register windows services
############################################################

Write-Host 'Removing old an installing new .NET LAB windows services...'

$healthcheckservicepath = Join-Path $location '\LabHealthCheckService\LabHealthCheckService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $healthcheckservicepath | out-null
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i $healthcheckservicepath

$wcfservicepath = Join-Path $location '\TrainingWCFService\TrainingWCFService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $wcfservicepath | out-null
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i $wcfservicepath

$cacheservicepath = Join-Path $location '\CacheService\CacheService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $cacheservicepath | out-null
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i $cacheservicepath

############################################################
# Deploy SQL database
############################################################

Write-Host 'Installing SQL Database for .NET Lab...'

$instance = Read-Host 'Enter SQL server instance name. Common values are localhost or localhost\sqlexpress ([enter] for localhost)'
if([string]::IsNullOrEmpty($instance))
{
	$instance = 'localhost'
}

SQLCMD -E -S $instance -i "LabDB.sql"

if($LASTEXITCODE -ne 0)
{
	Write-Host "Problems creating database. Script will not continue."
	Exit
}

# Update SQL connection string in all projects
Get-ChildItem -Path $location -Filter '*.config' -Recurse | 
Foreach-Object { 
	$text = [System.IO.File]::ReadAllText($_.FullName) 
	$newtext = $text.Replace('localhost\sqlexpress', $instance)
	[System.IO.File]::WriteAllText($_.FullName, $newtext) 
}

############################################################
# Health check the web site
############################################################



#Start services

Start-Service LabCacheServer
Start-Service TrainingLabWCFService
Start-Service LabHealthCheckService