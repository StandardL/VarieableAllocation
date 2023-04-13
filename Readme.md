# 安装指南

1. 在Release页面下载打包的软件并解压。
2. 在Windows设置 - 隐私和安全性 - 开发者选项中，开启开发者模式。

![image-20230413193444100](/images/Setting_Dev.png)

3. 使用PowerShell安装本程序。

   ![image-20230413193735249](/images/Install_Powershell.png)

   安装途中可能需要向您索取管理员权限，以安装来自作者的签名证书。

   当PowerShell终端出现如下字样时，表示已经安装成功，您可以在Windows开始菜单中启动或卸载。

   ![image-20230413194015430](/images/Install_Fin.png)

   

# GUI版本编译需求

(翻译自微软[开源文档](https://github.com/microsoft/WindowsAppSDK-Samples/blob/main/README.md))

该Windows App SDK应用需要满足以下的系统要求：

- Windows 10, version 1809 (build 17763) 或更新，64位架构.
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) 或 Visual Studio 2019 (16.9或更新), 并安装了以下组件
  - 通用 Windows 平台开发(Universal Windows Platform development)
  - .NET桌面开发 (.NET Desktop Development) (即使你只需要构建 C++ Win32 应用)
  - 使用 C++ 的桌面开发(Desktop development with C++) (即使你只需要构建 .NET 应用)
  - Windows SDK 版本2004 (build 19041) 或更新. 正常来说Visual Studio已默认安装了.

更多使用Windows App SDK开发信息请参考[系统需求]((https://docs.microsoft.com/windows/apps/windows-app-sdk/system-requirements) )文档和[Windows应用开发工具](https://docs.microsoft.com/windows/apps/windows-app-sdk/set-up-your-development-environment)文档.

