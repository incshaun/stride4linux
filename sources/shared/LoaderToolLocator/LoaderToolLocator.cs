// Copyright (c) .NET Foundation and Contributors (https://dotnetfoundation.org/ & https://stride3d.net)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System;
using System.IO;

namespace Stride.Core
{
    /// <summary>
    /// Helper class to find executable (usually in folder tools/TFM) from a .NET Core library (usually in folder lib/TFM).
    /// This follow the layout generated by Stride.NuGetLoader that is shipped with every .exe project such as GameStudio, Asset Compiler, Connection router, etc.
    /// </summary>
    static class LoaderToolLocator
    {
        internal static string GetExecutable(string assemblyLocation)
        {
            // Already exe
            if (Path.GetExtension(assemblyLocation).ToLowerInvariant() == ".exe")
                return assemblyLocation;

            // Same folder (dev or old layout in nuget package)
            {
                var exeLocation = Path.ChangeExtension(assemblyLocation, ".exe");
                if (File.Exists(exeLocation))
                    return exeLocation;
            }

            // Linux version of exe has no extension.
            {
                var exeLocation = Path.ChangeExtension(assemblyLocation, null);
                if (File.Exists(exeLocation))
                    return exeLocation;
            }

            // Remap lib\TFM to tools\TFM (nuget package)
            var tfmPath = Path.GetDirectoryName(assemblyLocation);
            if (tfmPath != null)
            {
                const string LibPath = "\\lib";

                var parentPath = Path.GetDirectoryName(tfmPath);
                if (parentPath.EndsWith(LibPath, StringComparison.Ordinal))
                {
                    // Replace lib to tools
                    var tfm = tfmPath.Substring(parentPath.Length + 1);
                    var toolPath = $"{assemblyLocation.Substring(0, parentPath.Length - LibPath.Length)}\\tools";
                    var exeFile = Path.ChangeExtension(assemblyLocation.Substring(tfmPath.Length + 1), ".exe");
                    var exeLocation = $"{toolPath}\\{tfm}\\{exeFile}";
                    if (File.Exists(exeLocation))
                        return exeLocation;
                }
            }

            throw new FileNotFoundException($"Could not find executable to start assembly [${assemblyLocation}]");
        }
    }
}
