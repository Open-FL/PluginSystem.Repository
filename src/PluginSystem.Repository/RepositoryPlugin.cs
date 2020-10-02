using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using PluginSystem.Core;
using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;
using PluginSystem.Utility;

namespace PluginSystem.Repository
{
    public class RepositoryPlugin : APlugin<PluginSystemHost>, IPluginHost
    {

        public static string GetOriginFilePath(PluginAssemblyPointer ptr)
        {
           return Path.Combine(PluginPaths.GetPluginConfigDirectory(ptr), "origins.txt");
        }

        public string OriginFile => GetOriginFilePath(PluginAssemblyData);

        bool IPluginHost.IsAllowedPlugin(IPlugin plugin)
        {
            return true;
        }

        void IPluginHost.OnPluginLoad(IPlugin plugin, BasePluginPointer ptr)
        {
        }

        void IPluginHost.OnPluginUnload(IPlugin plugin)
        {
        }


        private void EnsureOriginFileExists()
        {
            if (!File.Exists(OriginFile))
            {
                File.WriteAllText(OriginFile, "");
            }
        }

        public void AddOrigin(string origin)
        {
            EnsureOriginFileExists();
            List<string> origins = ListHelper.LoadList(OriginFile).ToList();
            origins.Add(origin);
            ListHelper.SaveList(OriginFile, origins.Distinct());
        }

        public void RemoveOrigin(string origin)
        {
            EnsureOriginFileExists();
            List<string> origins = ListHelper.LoadList(OriginFile).ToList();
            origins.Remove(origin);
            ListHelper.SaveList(OriginFile, origins);
        }

        public List<Repository> GetPlugins()
        {
            EnsureOriginFileExists();
            string[] origins = ListHelper.LoadList(OriginFile);
            List<Repository> repos = new List<Repository>();
            foreach (string origin in origins)
            {
                repos.Add(new Repository(origin, GetOriginData(origin)));
            }

            return repos;
        }


        private static List<string> ReadList(string uri)
        {
            if (File.Exists(uri))
            {
                return File.ReadAllLines(uri).ToList();
            }

            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(uri).Split('\n').ToList();
            }
        }

        private static List<BasePluginPointer> GetOriginData(string origin)
        {
            return ReadList(origin).SelectMany(x => ReadList(x.Trim()).Select(y => new BasePluginPointer(y.Trim())))
                                   .ToList();
        }

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);

            EnsureOriginFileExists();

            PluginManager.LoadPlugins(this);
        }

    }
}