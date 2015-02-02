﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Microsoft.Framework.Runtime
{
    internal class ProcessingQueue
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public event Action<Dictionary<string, int>> ProjectsInitialized;
        public event Action<int, CompileResponse> ProjectCompiled;
        public event Action<int> ProjectChanged;
        public event Action Closed;
        public event Action<IEnumerable<string>> ProjectSources;

        public ProcessingQueue(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        public void Start()
        {
            new Thread(ReceiveMessages).Start();
        }

        public void Send(DesignTimeMessage message)
        {
            lock (_writer)
            {
                _writer.Write(JsonConvert.SerializeObject(message));
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var messageType = _reader.ReadString();
                    if (messageType == "Assembly")
                    {
                        var compileResponse = new CompileResponse();
                        var id = _reader.ReadInt32();
                        var warningsCount = _reader.ReadInt32();
                        compileResponse.Warnings = new string[warningsCount];
                        for (int i = 0; i < warningsCount; i++)
                        {
                            compileResponse.Warnings[i] = _reader.ReadString();
                        }

                        var errorsCount = _reader.ReadInt32();
                        compileResponse.Errors = new CompileResponseError[errorsCount];
                        for (int i = 0; i < errorsCount; i++)
                        {
                            compileResponse.Errors[i] = new CompileResponseError
                            {
                                FilePath = _reader.ReadString(),
                                Message = _reader.ReadString(),
                                StartLine = _reader.ReadInt32(),
                                StartColumn = _reader.ReadInt32(),
                                EndLine = _reader.ReadInt32(),
                                EndColumn = _reader.ReadInt32()
                            };
                        }
                        var embeddedReferencesCount = _reader.ReadInt32();
                        compileResponse.EmbeddedReferences = new Dictionary<string, byte[]>();
                        for (int i = 0; i < embeddedReferencesCount; i++)
                        {
                            var key = _reader.ReadString();
                            int valueLength = _reader.ReadInt32();
                            var value = _reader.ReadBytes(valueLength);
                            compileResponse.EmbeddedReferences[key] = value;
                        }

                        var assemblyBytesLength = _reader.ReadInt32();
                        compileResponse.AssemblyBytes = _reader.ReadBytes(assemblyBytesLength);
                        var pdbBytesLength = _reader.ReadInt32();
                        compileResponse.PdbBytes = _reader.ReadBytes(pdbBytesLength);

                        ProjectCompiled(id, compileResponse);
                    }
                    else if(messageType == "Sources")
                    {
                        int count = _reader.ReadInt32();
                        var files = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            files.Add(_reader.ReadString());
                        }

                        ProjectSources(files);
                    }
                    else if (messageType == "ProjectContexts")
                    {
                        int count = _reader.ReadInt32();
                        var projectContexts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < count; i++)
                        {
                            string key = _reader.ReadString();
                            int id = _reader.ReadInt32();

                            projectContexts[key] = id;
                        }

                        ProjectsInitialized(projectContexts);
                    }
                    else if (messageType == "ProjectChanged")
                    {
                        var id = _reader.ReadInt32();
                        ProjectChanged(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("[{0}]: Exception occurred: {1}", GetType().Name, ex);
                Closed();
                return;
            }
        }
    }
}