// <auto-generated />
namespace Microsoft.Framework.DesignTimeHost
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.Framework.DesignTimeHost.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// Cannot process plugin message. Plugin id '{0}' must have a {1} of '{2}'.
        /// </summary>
        internal static string Plugin_CannotProcessMessageInvalidPluginType
        {
            get { return GetString("Plugin_CannotProcessMessageInvalidPluginType"); }
        }

        /// <summary>
        /// Cannot process plugin message. Plugin id '{0}' must have a {1} of '{2}'.
        /// </summary>
        internal static string FormatPlugin_CannotProcessMessageInvalidPluginType(object p0, object p1, object p2)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Plugin_CannotProcessMessageInvalidPluginType"), p0, p1, p2);
        }

        /// <summary>
        /// Unable to find project.json in '{0}'.
        /// </summary>
        internal static string Plugin_UnableToFindProjectJson
        {
            get { return GetString("Plugin_UnableToFindProjectJson"); }
        }

        /// <summary>
        /// Unable to find project.json in '{0}'.
        /// </summary>
        internal static string FormatPlugin_UnableToFindProjectJson(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Plugin_UnableToFindProjectJson"), p0);
        }

        /// <summary>
        /// Message received for unregistered plugin id '{0}'. Plugins must first be registered before they can receive messages.
        /// </summary>
        internal static string Plugin_UnregisteredPluginIdCannotReceiveMessages
        {
            get { return GetString("Plugin_UnregisteredPluginIdCannotReceiveMessages"); }
        }

        /// <summary>
        /// Message received for unregistered plugin id '{0}'. Plugins must first be registered before they can receive messages.
        /// </summary>
        internal static string FormatPlugin_UnregisteredPluginIdCannotReceiveMessages(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Plugin_UnregisteredPluginIdCannotReceiveMessages"), p0);
        }

        /// <summary>
        /// No plugin with id '{0}' has been registered. Cannot unregister plugin.
        /// </summary>
        internal static string Plugin_UnregisteredPluginIdCannotUnregister
        {
            get { return GetString("Plugin_UnregisteredPluginIdCannotUnregister"); }
        }

        /// <summary>
        /// No plugin with id '{0}' has been registered. Cannot unregister plugin.
        /// </summary>
        internal static string FormatPlugin_UnregisteredPluginIdCannotUnregister(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Plugin_UnregisteredPluginIdCannotUnregister"), p0);
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
