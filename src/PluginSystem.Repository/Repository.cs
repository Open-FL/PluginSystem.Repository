using System;
using System.Collections.Generic;

using PluginSystem.Core.Pointer;

namespace PluginSystem.Repository
{
    public class Repository
    {

        public readonly string RepositoryOrigin;
        public readonly IReadOnlyCollection<BasePluginPointer> Plugins;

        public Repository(string origin, List<BasePluginPointer> plugins)
        {
            RepositoryOrigin = origin;
            Plugins = plugins.AsReadOnly();
        }

    }
}
