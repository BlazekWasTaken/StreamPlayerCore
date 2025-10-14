# StreamPlayerCore

[![NuGet](https://img.shields.io/badge/NuGet-StreamPlayerCore.WPF.Control-004880?logo=nuget&logoColor=fff)](https://www.nuget.org/packages/StreamPlayerCore.WPF.Control/)
[![NuGet](https://img.shields.io/badge/NuGet-StreamPlayerCore.WinForms.Control-004880?logo=nuget&logoColor=fff)](https://www.nuget.org/packages/StreamPlayerCore.WinForms.Control/)

WPF and WinForms controls for video streaming using FFmpeg. Built with .Net9 and .Net Framework 4.8.

Based on the [WebEye](https://github.com/jacobbo/WebEye/tree/221e8187afdfaf0b5522848832199913f46b2b7d) library.

## Build instructions

### Requirements

- Git
- dotnet 9 SDK
- [MSVC - VS C++ x64/x86 build tools | MSVC - VS C++ x64/x86 Spectre-mitigated libs | Windows SDK](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022)
- Jetbrains Rider
- MSYS2

### Building libraries

1. Install all requirements.
2. Create a ```C:\StreamPlayerCore-build``` folder and navigate to it in a terminal.
3. Clone the FFmpeg repository for a 32-bit build:
   ```bash
   git clone --branch <version tag> https://github.com/FFmpeg/FFmpeg FFmpeg32
   ```
4. Clone the FFmpeg repository for a 64-bit build:
   ```bash
   git clone --branch <version tag> https://github.com/FFmpeg/FFmpeg FFmpeg64
   ```
5. Download the latest [Boost release](https://www.boost.org/releases/latest/)
6. Extract the Boost archive to ```C:\StreamPlayerCore-build\boost```. 
7. Download the latest [NASM release](https://www.nasm.us/) (Executable only), extract it to ```C:\StreamPlayerCore-build``` and rename to ```nasm.exe```. 
8. Add ```C:\StreamPlayerCore-build``` to your system PATH. 
9. Create an ```MSYS2_PATH_TYPE``` environment variable with the value ```inherit```.
10. Navigate to ```C:\StreamPlayerCore-build\boost``` and run:
   ```bash
   .\bootstrap.bat
   ```
   and then:
   ```bash
   .\b2 runtime-link=static
   ```
11. Open the ```x86 Native Tools Command Prompt for VS 2022```
12. Navigate to ```C:\msys64``` and open the ```msys2_shell.cmd``` file.
13. run 
    ```bash
    pacman -Syu pkg-config diffutils make
    ```
14. Navigate to ```C:\StreamPlayerCore-build\FFmpeg32``` and run:
    ```bash
    ./configure --toolchain=msvc --arch=i686 --enable-version3 --enable-static --disable-shared --disable-programs --disable-doc
    ```
    and then:
    ```bash
    make
    ```
15. Open the ```x64 Native Tools Command Prompt for VS 2022```
16. Navigate to ```C:\msys64``` and open the ```msys2_shell.cmd``` file.
17. Navigate to ```C:\StreamPlayerCore-build\FFmpeg64``` and run:
    ```bash
    ./configure --toolchain=msvc --arch=amd64 --target-os=win64 --enable-version3 --enable-static --disable-shared --disable-programs --disable-doc
    ```
    and then:
    ```bash
    make
    ```
    
### Building StreamPlayerCore

1. Clone the StreamPlayerSharp repository:
   ```bash
    git clone https://github.com/BlazekWasTaken/StreamPlayerCore
    ```
2. Open the ```StreamPlayerCore.sln``` file in Jetbrains Rider.
3. Open the ```StreamPlayer``` project and edit the ```StreamPlayer.vcxproj```. Replace:
   ```
    C:\Program Files (x86)\Windows Kits\10\Lib\10.0.26100.0\um\x86
   ```
   with the path to your Windows SDK x86 libraries. And:
   ```
    C:\Program Files (x86)\Windows Kits\10\Lib\10.0.26100.0\um\x64
   ```
   with the path to your Windows SDK x64 libraries.
4. Change the ```StreamPlayer``` build configuration to ```Release | Win32```.
5. Build the project.
6. Change the ```StreamPlayer``` build configuration to ```Release | x64```.
7. Build the project again.
8. Open the ```StreamPlayerCore.WinForms.Control``` project and open the ```Resources.resx``` file. This will remove the lingering errors.
9. Build the project.
10. Open the ```StreamPlayerCore.WPF.Control``` project and open the ```Resources.resx``` file. This will remove the lingering errors.
11. Build the project.

### Testing

1. Run the ```StreamPlayerCore.WinForms.Demo``` project.
2. Run the ```StreamPlayerCore.WPF.Demo``` project.

### Tested versions
- Dotnet sdk - 9.0.305
- Windows SDK - 10.0.26100.4654
- MSVC - v143 - VS 2022
- MSYS2 - 20250830
- Boost - 1.89.0
- FFmpeg - 8.0
- NASM - 3.0.1