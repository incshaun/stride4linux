# Get rid of all old packages.

rm -r ~/.nuget/packages/stride.*
rm ../bin/packages/Stride.*

# Build the editor.
(cd editor/Stride.GameStudio.Avalonia; dotnet clean Stride.GameStudio.Avalonia.csproj )
(cd editor/Stride.GameStudio.Avalonia; dotnet build /nologo /nr:false /m /verbosity:n /p:Configuration=Release /p:Platform="Linux" /p:StrideSkipUnitTests=false Stride.GameStudio.Avalonia.csproj /p:DeployExtension=false)
cp -r editor/Stride.GameStudio.Avalonia/bin/Release/net8.0 ~/.nuget/packages/stride.core.assets.compilerapp/4.2.0.2122/lib/
cp -r editor/Stride.GameStudio.Avalonia/bin/Release/net8.0/Stride.Core.Assets.CompilerApp ~/.nuget/packages/stride.core.assets.compilerapp/4.2.0.2122/lib/net8.0/Stride.Core.Assets.CompilerApp.exe
cp editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0-windows7.0/DxtWrapper.so ~/.nuget/packages/stride.core.assets.compilerapp/4.2.0.2122/lib/net8.0/
cp editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0-windows7.0/DxtWrapper.so ~/.nuget/packages/stride.core.assets.compilerapp/4.2.0.2122/lib/net8.0/
cp editor/Stride.GameStudio.Avalonia/bin/Debug/net8.0-windows7.0/runtimes/linux-x64/native/lib* ~/.nuget/packages/stride.core.assets.compilerapp/4.2.0.2122/lib/net8.0/

# May need to patch in loading the presentation package to run the editor.

# Build the android content.
dotnet build ../build/Stride.Android.sln /p:Configuration=Release

# Build the test project
# Clear
(cd ~/"Documents/Stride Projects/MyGame4"; rm -r Bin/ MyGame4/obj/ MyGame4/bin/ MyGame4.Linux/obj/ MyGame4.Android/obj)

# Build Linux executable.
(cd ~/"Documents/Stride Projects/MyGame4"; dotnet build /p:Configuration=Release  MyGame4.Linux/MyGame4.Linux.csproj)

# Build android apk
(cd ~/"Documents/Stride Projects/MyGame4"; dotnet build /p:Configuration=Release  MyGame4.Android/MyGame4.Android.csproj /p:PackageFormat=Apk)
