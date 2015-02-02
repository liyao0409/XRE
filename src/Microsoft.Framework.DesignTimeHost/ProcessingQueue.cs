// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Framework.DesignTimeHost.Models;
using Newtonsoft.Json;

namespace Microsoft.Framework.DesignTimeHost
{
    public class ProcessingQueue
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public event Action<Message> OnReceive;

        public ProcessingQueue(Stream stream)
        {
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        public void Start()
        {
            Trace.TraceInformation("[ProcessingQueue]: Start()");
            new Thread(ReceiveMessages).Start();
        }

        public bool Send(Action<BinaryWriter> write)
        {
            try
            {
                lock (_writer)
                {
                    write(_writer);
                    return true;
                }
            }
            catch (IOException)
            {
                // Swallow those
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("[ProcessingQueue]: Error sending {0}", ex);
            }

            return false;
        }

        public bool Send(Message message)
        {
            lock (_writer)
            {
                try
                {
                    Trace.TraceInformation("[ProcessingQueue]: Send({0})", message);
                    _writer.Write(JsonConvert.SerializeObject(message));

                    return true;
                }
                catch (IOException)
                {
                    // Swallow those
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation("[ProcessingQueue]: Error sending {0}", ex);
                }
            }

            return false;
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var payload = _reader.ReadString();
                    var message = JsonConvert.DeserializeObject<Message>(payload);
                    Trace.TraceInformation("[ProcessingQueue]: OnReceive({0})", message);
                    OnReceive(message);
                }
            }
            catch (IOException)
            {
                // Swallow those
            }
            catch (Exception ex)
            {
                Trace.TraceInformation("[ProcessingQueue]: Error occurred: {0}", ex);
            }
        }
    }
}
