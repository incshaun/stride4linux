Install .NET
------------

https://dotnet.microsoft.com/en-us/download/dotnet/8.0

wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0
  
Building parts of the engine
----------------------------

Round 1

Stride.Code.Tests
Stride.Core.AssemblyProcessor
Stride.Core.AssemblyProcessor.Packed
Stride.Core.AssemblyProcessor.Tests
Stride.Core.Assets
Stride.Core.Assets.Tests
Stride.Core.BuildEngine.Common
Stride.Core.BuildEngine.Tests
Stride.Core.CompilerServices
Stride.Core.CompilerServices.Tests
Stride.Core.Design
Stride.Core.Design.Tests
Stride.Core
Stride.Core.IO
Stride.Core.Mathematics
Stride.Core.Mathematics.Tests
Stride.Core.MicroThreading
Stride.Core.Packages
Stride.Core.Presentation
Stride.Core.ProjectTemplating
Stride.Core.ProjectTemplating.Tests
Stride.Core.Quantum
Stride.Core.Quantum.Tests
Stride.Core.Reflection
Stride.Core.Serialization
Stride.Core.Shaders
Stride.Core.Tasks
Stride.Core.Tests
Stride.Core.Translation
Stride.Core.Yaml
Stride.Core.Yaml.Tests
Stride
Stride.FixProjectReferences
Stride.FontCompiler
Stride.Graphics
Stride.Graphics.RenderDocPlugin
Stride.Irony
Stride.Metrics
Stride.NuGetResolver
Stride.Samples.Templates
Stride.Shaders.Compiler
Stride.Shaders
Stride.Shaders.Parser
Stride.StorageTool
Stride.TextureConverter
Stride.VisualStudio.Commands.Interfaces

Round 2

Stride.Core.Assets.Quantum - remove WPF
Stride.Core.Assets.Quantum.Tests
Stride.Audio
Stride.Debugger
Stride.Engine
Stride.Engine.NoAssets.Tests
Stride.Games
Stride.Input
Stride.Native
Stride.Navigation
Stride.Particles
Stride.Physics
Stride.Rendering
Stride.UI
Stride.Video
Stride.VirtualReality
Stride.Voxels
Stride.Core.Translation.Presentation




Dependencies:

All in core complete.
All in assets - Stride.Core.Assets.Yaml maybe.
All in buildengine - Stride.Core.BuildEngine seems inactive
Mostly checked in engine.


Stride.ConnectionRouter - disabled sdk targets.
Stride.VirtualReality - requires some native compilation. Fixed in Stride.Native.targets.
Stride.Core.Assets.CompilerApp - native dependencies. Now failing on FBX dependency.
Stride.Core.BuildEngine - depends on Stride.Framework, does not appear to be active.
Stride.Assets.Models - disabled FBX.
Stride.Assets.Tests2 - issue, ignoring for the moment.
Stride.Core.Presentation.Dialogs - move some files out of Wpf, comment out some window dependencies.
Stride.Core.Presentation.Quantum - also leaking some Wpf dependencies.

Native dependencies:
- use clang
- sometime add: -fdeclspec
- edit sources/native/../../deps/NativePath/standard/../NativePath.h, and comment out the 2 macros that flag #error
- will need to change build targets at some point.

dotnet add package System.Resources.Extensions --version 8.0.0
dotnet build Stride.GameStudio.Avalonia.csproj  --runtime linux-x64 -p StridePlatforms=Linux


./bin/Debug/net8.0-windows7.0/Stride.GameStudio.Avalonia 


