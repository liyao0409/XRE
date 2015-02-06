// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Framework.FileSystemGlobbing;
using Microsoft.Framework.FileSystemGlobbing.Abstractions;

namespace Microsoft.Framework.Runtime.FileGlobbing
{
    public static class MatcherExtensions
    {
        public static void AddExcludePatterns(this Matcher self, params IEnumerable<string>[] excludePatternsGroups)
        {
            foreach (var group in excludePatternsGroups)
            {
                foreach (var pattern in group)
                {
                    self.AddExclude(pattern);
                }
            }
        }

        public static void AddIncludePatterns(this Matcher self, params IEnumerable<string>[] includePatternsGroups)
        {
            foreach (var group in includePatternsGroups)
            {
                foreach (var pattern in group)
                {
                    self.AddInclude(pattern);
                }
            }
        }

        public static string[] GetResultsInFullPath(this Matcher self, string directoryPath)
        {
            var relativePaths = self.Execute(new DirectoryInfoWrapper(new DirectoryInfo(directoryPath))).Files;
            var result = relativePaths.Select(path => Path.Combine(directoryPath, path)).ToArray();

            return result;
        }
    }
}