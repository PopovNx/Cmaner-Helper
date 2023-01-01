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

echo "Cmaner-Helper installed successfully!"