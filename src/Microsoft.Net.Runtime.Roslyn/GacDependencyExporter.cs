﻿using System.Runtime.Versioning;
using Microsoft.Net.Runtime.Loader;
using NuGet;

namespace Microsoft.Net.Runtime.Roslyn
{
    public class GacDependencyExporter : IDependencyExporter
    {
        private readonly IGlobalAssemblyCache _globalAssemblyCache;

        public GacDependencyExporter(IGlobalAssemblyCache globalAssemblyCache)
        {
            _globalAssemblyCache = globalAssemblyCache;
        }

        public IDependencyExport GetDependencyExport(string name, FrameworkName targetFramework)
        {
            // Only use the GAC on full .NET
            if (targetFramework.Identifier == VersionUtility.DefaultTargetFramework.Identifier)
            {
                string assemblyLocation;
                if (_globalAssemblyCache.TryResolvePartialName(name, out assemblyLocation))
                {
                    return new DependencyExport(assemblyLocation);
                }
            }

            return null;
        }
    }
}