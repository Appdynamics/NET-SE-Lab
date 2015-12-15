Function Test-IsAdmin
{
	If (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(`
	    [Security.Principal.WindowsBuiltInRole] "Administrator"))
	{
	    Write-Warning "You do not have Administrator rights to run this script!`nPlease re-run this script as an Administrator (Run as Administrator)!"
	    Break
	}
}


Function Test-PowerShellVersion
{
    # If version is less than 3.0 give a warning
    if($PSVersionTable.PSVersion.Major -lt 3)
    {
        Write-Warning "Current script requires PowerShell version 3.0 or later.`nPlease upgrade the PowerShell and re-run this script."
	    Break
    }
}

Function Test-Net45
{
    $path = 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'

	if (Test-Path $path)
	{
		if (Get-ItemProperty $path -Name Version -ErrorAction SilentlyContinue)
		{
			$reg = Get-ItemProperty $path
			$v = [version]$reg.Version
			$v45 = [version]"4.5.0"
			if($v -ge $v45)
			{
				return
			}
		}
	}

    Write-Warning "Current lab requires .NET 4.5+ Full installed.`nPlease install it from internet and re-run this script."
	Break
}

Function Test-SQL
{
    $sql = Get-Command "sqlcmd.exe" -ErrorAction SilentlyContinue

    if($sql -eq $null)
    {
		Write-Warning "SQLCMD.exe was not found.`nPlease Install local SQL or SQL Express, then close and relaunch PowerShell."
		Break
    }

    $instance = Read-Host 'Enter SQL server instance name. Common values are localhost or localhost\sqlexpress ([enter] for localhost)'
	if([string]::IsNullOrEmpty($instance))
	{
		$instance = 'localhost'
	}

	SQLCMD -E -S $instance -Q "SELECT @@VERSION AS 'SQL Server Version'"

    if($LASTEXITCODE -ne 0)
    {
		Write-Warning "There was an error connecting to SQL server.`nPlease fix it and re-run this script."
		Break
    }
}


Test-IsAdmin
Test-PowerShellVersion
Test-Net45
Test-Sql


Write-Host ([Environment]::NewLine)
Write-Host "OK, good to go! Continue installing the lab."
