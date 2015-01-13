﻿using System.Collections.Generic;

namespace Microsoft.Framework.Runtime
{
    public class CompileResponse
    {
        public IList<CompileResponseError> Errors { get; set; }
        public IList<string> Warnings { get; set; }

        public IDictionary<string, byte[]> EmbeddedReferences { get; set; }

        public byte[] AssemblyBytes { get; set; }
        public byte[] PdbBytes { get; set; }

        public string AssemblyPath { get; set; }
    }
}