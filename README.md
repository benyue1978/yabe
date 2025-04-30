# Yet Another BACnet Explorer MacOS 

Migrate from <https://sourceforge.net/projects/yetanotherbacnetexplorer/>

For learning purpose. 

This project consists of two main components:

- **DemoServer**: A BACnet protocol simulation server that supports BACnet/IP device simulation and testing.
- **Yabe.Avalonia**: A cross-platform BACnet client GUI based on Avalonia, supporting device discovery, object browsing, property reading, etc.

---

## Requirements

- .NET 9.0 SDK or above
- Recommended OS: macOS (Windows/Linux supported but not fully tested)

---

## 1. Running DemoServer (BACnet Simulation Server)

### Build

```sh
# In project root directory
cd DemoServer
# Build
dotnet build
```

### Run

```sh
# Go to output directory
cd bin/Debug/net9.0
# Start DemoServer
./DemoServer
```

DemoServer listens on UDP port 0xBAC0, simulating a BACnet device that supports client discovery, property reading, and other BACnet operations.

---

## 2. Running Yabe.Avalonia (Cross-platform BACnet Client)

### Build UI

```sh
# In project root directory
cd yabe.avalonia
# Build
dotnet build
```

### Run UI

```sh
# Run Avalonia UI client
dotnet run
```

### Features

- Click "Discover Devices" to automatically search for BACnet devices (like DemoServer) in the local network.
- Click on a device to browse its object tree and properties.
- Log area displays real-time operation and communication logs.

---

## 3. Troubleshooting

- For port occupation or permission issues, check firewall settings or run with administrator privileges.
- DemoServer reads DeviceStorage.xml configuration file by default, ensure it exists in the output directory.
- Yabe.Avalonia depends on yabe.core project, no separate compilation needed.

---

## 4. References

- Avalonia Documentation: <https://avaloniaui.net/>
- .NET Documentation: <https://learn.microsoft.com/dotnet/>
- BACnet Protocol Resources: <https://www.bacnet.org/>

---

Add a wireshark Lua script to dissect BACnet packages from dynamic UDP ports.

<https://wiki.wireshark.org/lua/dissectors>

Usage: drop the lua script into the wireshark plugin directory

```shell
mkdir -p ~/.config/wireshark/plugins
cp bacnet_wireshark.lua ~/.config/wireshark/plugins
```
