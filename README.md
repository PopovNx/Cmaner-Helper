# Cmaner-Helper
Tool for quick and convenient calling of commands

# Instructions

## Setup & Configuration

### Linux install

To install run next script:
```
wget https://raw.githubusercontent.com/PopovDev/Cmaner-Helper/main/install-cm.sh
chmod +x install-cm.sh
./install-cm.sh
```

For uninstall:
```
./install-cm.sh uninstall
```

### Windows install

To install run next script in powershell:
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex "&{$((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/PopovDev/Cmaner-Helper/main/windows-install.ps1'))}"
```
For uninstall:
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex "&{$((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/PopovDev/Cmaner-Helper/main/windows-install.ps1'))} uninstall"
```
