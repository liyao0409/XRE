// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Framework.DesignTimeHost.Models.IncomingMessages;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Common.DependencyInjection;

namespace Microsoft.Framework.DesignTimeHost
{
    public class PluginHandler
    {
        private readonly Action<object> _sendMessageMethod;
        private readonly IServiceProvider _hostServices;
        private readonly Dictionary<int, IPlugin> _plugins;

        public PluginHandler(IServiceProvider hostServices, Action<object> sendMessageMethod)
        {
            _sendMessageMethod = sendMessageMethod;
            _hostServices = hostServices;
            _plugins = new Dictionary<int, IPlugin>();
        }

        public void ProcessMessage(PluginMessage data, Lazy<IAssemblyLoadContext> assemblyLoadContext)
        {
            switch (data.MessageName)
            {
                case "Register":
                    ProcessRegisterMessage(data, assemblyLoadContext);
                    break;
                case "Unregister":
                    ProcessUnregisterMessage(data);
                    break;
                case "PluginMessage":
                    ProcessPluginMessage(data);
                    break;
            }
        }

        private void ProcessPluginMessage(PluginMessage data)
        {
            IPlugin plugin;
            if (_plugins.TryGetValue(data.PluginId, out plugin))
            {
                plugin.ProcessMessage(data.Data);
            }
            else
            {
                throw new InvalidOperationException(
                    Resources.FormatPlugin_UnregisteredPluginIdCannotReceiveMessages(data.PluginId));
            }
        }

        private void ProcessUnregisterMessage(PluginMessage data)
        {
            if (_plugins.ContainsKey(data.PluginId))
            {
                _plugins.Remove(data.PluginId);
            }
            else
            {
                throw new InvalidOperationException(
                    Resources.FormatPlugin_UnregisteredPluginIdCannotUnregister(data.PluginId));
            }
        }

        private IPlugin ProcessRegisterMessage(
            PluginMessage data,
            Lazy<IAssemblyLoadContext> assemblyLoadContext)
        {
            var pluginId = data.PluginId;

            var registerData = data.Data.ToObject<PluginRegisterData>();

            // REVIEW: Should we catch errors that result in bad assembly loads/GetTypes?
            var assembly = assemblyLoadContext.Value.Load(registerData.AssemblyName);
            var pluginType = assembly.GetType(registerData.TypeName);

            IPlugin plugin = null;

            if (pluginType != null)
            {
                // We build out a custom plugin service provider to add a PluginMessageBroker to the potential
                // services that can be used to construct an IPlugin.
                var pluginServiceProvider = new PluginServiceProvider(
                    _hostServices,
                    messageBroker: new Lazy<PluginMessageBroker>(
                        () => new PluginMessageBroker(pluginId, _sendMessageMethod)));

                plugin = ActivatorUtilities.CreateInstance(pluginServiceProvider, pluginType) as IPlugin;

                if (plugin == null)
                {
                    throw new InvalidOperationException(
                        Resources.FormatPlugin_CannotProcessMessageInvalidPluginType(
                            pluginId, nameof(Type), typeof(IPlugin).FullName));
                }

                _plugins[pluginId] = plugin;
            }

            return plugin;
        }

        private class PluginServiceProvider : IServiceProvider
        {
            private static readonly TypeInfo MessageBrokerTypeInfo = typeof(IPluginMessageBroker).GetTypeInfo();
            private readonly IServiceProvider _hostServices;
            private readonly Lazy<PluginMessageBroker> _messageBroker;

            public PluginServiceProvider(IServiceProvider hostServices, Lazy<PluginMessageBroker> messageBroker)
            {
                _hostServices = hostServices;
                _messageBroker = messageBroker;
            }

            public object GetService(Type serviceType)
            {
                if (MessageBrokerTypeInfo.IsAssignableFrom(serviceType.GetTypeInfo()))
                {
                    return _messageBroker.Value;
                }
                else
                {
                    return _hostServices.GetService(serviceType);
                }
            }
        }

        private class PluginRegisterData
        {
            public string AssemblyName { get; set; }
            public string TypeName { get; set; }
        }
    }
}