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

Remove-WebSite 'LabWebSite' -ErrorAction SilentlyContinue
Remove-WebSite 'ApiWebSite' -ErrorAction SilentlyContinue
Remove-WebSite 'HealthCheckWebSite' -ErrorAction SilentlyContinue
Remove-WebAppPool 'DotNetLabAppPool' -ErrorAction SilentlyContinue

Stop-Service LabCacheServer
Stop-Service TrainingLabWCFService
Stop-Service LabHealthCheckService

$location = Get-ScriptDirectory

$healthcheckservicepath = Join-Path $location '\LabHealthCheckService\LabHealthCheckService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $healthcheckservicepath

$wcfservicepath = Join-Path $location '\TrainingWCFService\TrainingWCFService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $wcfservicepath

$cacheservicepath = Join-Path $location '\CacheService\CacheService.exe'
& $env:SystemRoot\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u $cacheservicepath
