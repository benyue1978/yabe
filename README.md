# Yabe BACnet 跨平台工具

从<https://sourceforge.net/projects/yetanotherbacnetexplorer/> 迁移到MacOS

仅作为学习使用

本项目包含两个主要组件：

- **DemoServer**：BACnet 协议模拟服务器，支持 BACnet/IP 设备模拟与测试。
- **Yabe.Avalonia**：基于 Avalonia 的跨平台 BACnet 客户端图形界面，可用于设备发现、对象浏览、属性读取等。

---

## 环境要求

- .NET 9.0 SDK 及以上
- 推荐操作系统：macOS（也支持 Windows/Linux，未经测试）

---

## 1. 运行 DemoServer（BACnet 模拟服务器）

### 编译

```sh
# 在项目根目录下
cd DemoServer
# 构建
 dotnet build
```

### 运行

```sh
# 进入输出目录
cd bin/Debug/net9.0
# 启动 DemoServer
./DemoServer
```

DemoServer 会监听 UDP 0xBAC0 端口，模拟一个 BACnet 设备，支持 BACnet 客户端发现、属性读取等操作。

---

## 2. 运行 Yabe.Avalonia（跨平台 BACnet 客户端）

### 编译UI

```sh
# 在项目根目录下
cd yabe.avalonia
# 构建
 dotnet build
```

### 运行UI

```sh
# 运行 Avalonia UI 客户端
 dotnet run
```

### 功能说明

- 点击“发现设备”可自动搜索局域网内 BACnet 设备（如 DemoServer）。
- 点击设备可浏览其对象树和属性。
- 日志区实时显示操作和通信日志。

---

## 3. 常见问题

- 如遇端口占用、权限等问题，请检查防火墙设置或以管理员权限运行。
- DemoServer 默认读取 DeviceStorage.xml 配置文件，确保该文件存在于输出目录。
- Yabe.Avalonia 依赖 yabe.core 项目，无需单独编译。

---

## 4. 参考

- Avalonia 官方文档：<https://avaloniaui.net/>
- .NET 官方文档：<https://learn.microsoft.com/dotnet/>
- BACnet 协议资料：<https://www.bacnet.org/>

---

Add a wireshark Lua script to dissect BACnet packages from dynamic UDP ports.

<https://wiki.wireshark.org/lua/dissectors>

Usage: drop the lua script into the wireshark plugin directory

```shell
mkdir -p ~/.config/wireshark/plugins
cp bacnet_wireshark.lua ~/.config/wireshark/plugins
```
