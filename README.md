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

#### Windows install

To install run next script in powershell:
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex "&{$((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/PopovDev/Cmaner-Helper/main/windows-install.ps1'))}"
```
For uninstall:
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex "&{$((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/PopovDev/Cmaner-Helper/main/windows-install.ps1'))} uninstall"
```

## Basic Usage
![image](https://user-images.githubusercontent.com/50965570/210394421-c5fbe260-6669-45d9-811b-2efa473c7a79.png)
Type "cm" to access an interactive menu of commands categorized

![image](https://user-images.githubusercontent.com/50965570/210390989-e4ecbf8a-5356-47ed-bdab-8c09bd884332.png)
By following the instructions, you can add commands by dividing them into categories. The command can be given a short call and the need for administrator rights. If a command requires administrator rights, on Linux it will be called via sudo, and on Windows via creating a new window with administrator rights

![image](https://user-images.githubusercontent.com/50965570/210394280-48c7cf19-73eb-4555-ac8e-2184f34805a6.png)

"cm" - can pass arguments through short calls, giving new life to simple python and other scripts

