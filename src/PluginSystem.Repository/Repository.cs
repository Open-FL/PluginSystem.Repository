using System.Collections.Generic;

using PluginSystem.Core.Pointer;

namespace PluginSystem.Repository
{
    public class Repository
    {

        public readonly IReadOnlyCollection<BasePluginPointer> Plugins;

        public readonly string RepositoryOrigin;

        public Repository(string origin, List<BasePluginPointer> plugins)
        {
            RepositoryOrigin = origin;
            Plugins = plugins.AsReadOnly();
        }

    }
}