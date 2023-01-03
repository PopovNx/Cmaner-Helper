#!/bin/bash
if [ "$1" = "uninstall" ]; then
    echo "Uninstalling Cmaner-Helper..."
    sudo rm /usr/bin/cm
    echo "Cmaner-Helper uninstalled successfully!"
    exit
fi

wget https://github.com/PopovDev/Cmaner-Helper/releases/latest/download/linux.x64.tar.gz
tar -xvf linux.x64.tar.gz
rm linux.x64.tar.gz
sudo cp linux.x64/cm /usr/bin/cm
rm -rf linux.x64
cm @initialize@
cm about

echo "Cmaner-Helper initialized successfully!"
echo "In some cases, you may need to restart your terminal to use Cmaner-Helper."
echo ""
