$ErrorActionPreference = "Stop"
if ($args[0] -eq "uninstall")
{
    Write-Host "Uninstalling Cmaner-Helper..."
    Remove-Item -Path "$env:USERPROFILE/.cmbin" -Recurse -Force
    Write-Host "Cmaner-Helper uninstalled successfully!"
}
else
{

    $psq = "https://github.com/PopovDev/Cmaner-Helper/releases/latest/download/windows.x64.zip";
    $tempZip = ".\cmzip.zip"
    $dirdis = "$env:USERPROFILE\.cmbin"
    Write-Host "Downloading Cmaner-Helper..."
    Invoke-WebRequest -Uri $psq -OutFile $tempZip | Out-Null
    Expand-Archive -Path $tempZip -DestinationPath .\ -Force | Out-Null
    Remove-Item -Path $tempZip | Out-Null
    New-Item -ItemType Directory -Path $dirdis -Force | Out-Null
    Copy-Item -Path .\windows.x64\cm.exe -Destination $dirdis\cm.exe -Force | Out-Null
    Remove-Item -Path .\windows.x64 -Recurse -Force | Out-Null
    $env:PATH = "$env:USERPROFILE\.cmbin;$env:PATH"
    [System.Environment]::SetEnvironmentVariable("PATH", $env:PATH, "User")
    Write-Host "Cmaner-Helper installed successfully!"
    &$dirdis\cm.exe @initialize@ | Out-Null
    &$dirdis\cm.exe about -Wait | Write-Host
    Write-Host "Cmaner-Helper initialized successfully!"
    Write-Host "In some cases, you may need to restart your terminal to use Cmaner-Helper."
    Write-Host ""
}
