$ErrorActionPreference = "Stop"
if ($args[0] -eq "uninstall") {
    Write-Output "Uninstalling Cmaner-Helper..."
    Remove-Item -Path "$env:USERPROFILE/.cmbin" -Recurse -Force
    Write-Output "Cmaner-Helper uninstalled successfully!"
}else{

$psq = "https://github.com/PopovDev/Cmaner-Helper/releases/download/1.1.0/windows.x64.zip";
$tempZip = ".\cmzip.zip"
$dirdis = "$env:USERPROFILE\.cmbin"
Invoke-WebRequest -Uri $psq -OutFile $tempZip
Expand-Archive -Path $tempZip -DestinationPath .\ -Force
Remove-Item -Path $tempZip
New-Item -ItemType Directory -Path $dirdis -Force
Copy-Item -Path .\windows.x64\cm.exe -Destination $dirdis\cm.exe -Force
Remove-Item -Path .\windows.x64 -Recurse -Force
$env:PATH = "$env:USERPROFILE\.cmbin;$env:PATH"
[System.Environment]::SetEnvironmentVariable("PATH", $env:PATH, "User")
Write-Output "$dirdis;$env:PATH"
Write-Output "Cmaner-Helper installed successfully!"
}
