# Install IIS using PowerShell - tested on windows 2012

Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Force

#Import-Module ServerManager
#Add-WindowsFeature Application-Server
#Add-WindowsFeature Web-Server
#Add-WindowsFeature Web-Mgmt-Tools
#Add-WindowsFeature WAS
#Add-WindowsFeature Web-App-Dev
#Add-WindowsFeature Web-Asp-Net45 -ErrorAction SilentlyContinue
#Add-WindowsFeature net-wcf-http-activation45 -ErrorAction SilentlyContinue
#Add-WindowsFeature net-wcf-tcp-activation45 -ErrorAction SilentlyContinue
#Add-WindowsFeature AS-TCP-Activation
#Add-WindowsFeature AS-HTTP-Activation
#Add-WindowsFeature NET-Non-HTTP-Activ

dism /Online /Enable-Feature /FeatureName:IIS-WebServerRole 
dism /Online /Enable-Feature /FeatureName:IIS-WebServer 
dism /Online /Enable-Feature /FeatureName:IIS-CommonHttpFeatures 
dism /Online /Enable-Feature /FeatureName:IIS-StaticContent 
dism /Online /Enable-Feature /FeatureName:IIS-DefaultDocument 
dism /Online /Enable-Feature /FeatureName:IIS-DirectoryBrowsing 
dism /Online /Enable-Feature /FeatureName:IIS-HttpErrors 
dism /Online /Enable-Feature /FeatureName:IIS-HttpRedirect 
dism /Online /Enable-Feature /FeatureName:IIS-ApplicationDevelopment 
dism /Online /Enable-Feature /FeatureName:IIS-ISAPIExtensions 
dism /Online /Enable-Feature /FeatureName:IIS-ISAPIFilter
dism /Online /Enable-Feature /FeatureName:NetFx4ServerFeatures 
dism /Online /Enable-Feature /FeatureName:NetFx4 
dism /Online /Enable-Feature /FeatureName:NetFx4Extended-ASPNET45
dism /Online /Enable-Feature /FeatureName:IIS-NetFxExtensibility
dism /Online /Enable-Feature /FeatureName:IIS-NetFxExtensibility45 
dism /Online /Enable-Feature /FeatureName:IIS-HealthAndDiagnostics 
dism /Online /Enable-Feature /FeatureName:IIS-HttpLogging 
dism /Online /Enable-Feature /FeatureName:IIS-Security 
dism /Online /Enable-Feature /FeatureName:IIS-RequestFiltering 
dism /Online /Enable-Feature /FeatureName:IIS-Performance 
dism /Online /Enable-Feature /FeatureName:IIS-WebServerManagementTools 
dism /Online /Enable-Feature /FeatureName:IIS-ManagementConsole 
dism /Online /Enable-Feature /FeatureName:IIS-ASPNET 
dism /Online /Enable-Feature /FeatureName:IIS-ASPNET45 
dism /Online /Enable-Feature /FeatureName:WAS-WindowsActivationService 
dism /Online /Enable-Feature /FeatureName:WAS-ProcessModel 
dism /Online /Enable-Feature /FeatureName:WAS-NetFxEnvironment 
dism /Online /Enable-Feature /FeatureName:WAS-ConfigurationAPI 
dism /Online /Enable-Feature /FeatureName:WCF-Services45 
dism /Online /Enable-Feature /FeatureName:WCF-HTTP-Activation45 
dism /Online /Enable-Feature /FeatureName:WCF-TCP-Activation45 
dism /Online /Enable-Feature /FeatureName:WCF-TCP-PortSharing45 
dism /Online /Enable-Feature /FeatureName:Application-Server 
dism /Online /Enable-Feature /FeatureName:AS-NET-Framework 
dism /Online /Enable-Feature /FeatureName:Application-Server-TCP-Port-Sharing 
dism /Online /Enable-Feature /FeatureName:Application-Server-WAS-Support 
dism /Online /Enable-Feature /FeatureName:Application-Server-HTTP-Activation 
dism /Online /Enable-Feature /FeatureName:Application-Server-TCP-Activation 
dism /Online /Enable-Feature /FeatureName:WCF-HTTP-Activation 
dism /Online /Enable-Feature /FeatureName:WCF-NonHTTP-Activation 
 



#ensure required windows features are installed
Import-Module Dism
Enable-WindowsOptionalFeature -Online -FeatureName WCF-HTTP-Activation
Enable-WindowsOptionalFeature -Online -FeatureName WCF-NonHTTP-Activation
Enable-WindowsOptionalFeature -Online -FeatureName WCF-Services45
Enable-WindowsOptionalFeature -Online -FeatureName WCF-HTTP-Activation45
Enable-WindowsOptionalFeature -Online -FeatureName WCF-TCP-Activation45
Enable-WindowsOptionalFeature -Online -FeatureName WCF-Pipe-Activation45
Enable-WindowsOptionalFeature -Online -FeatureName WCF-MSMQ-Activation45
Enable-WindowsOptionalFeature -Online -FeatureName WCF-TCP-PortSharing45

#ensure that NetTcpPortSharing service is set to automatic, and is started
Set-Service NetTcpPortSharing -StartupType Automatic
Start-Service NetTcpPortSharing



# Register .NET 4.0 on IIS prior to Windows 2012
if([System.Environment]::OSVersion.Version -lt [System.Version]::Parse("6.2"))
{
	& "$env:windir\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe" -i
}
