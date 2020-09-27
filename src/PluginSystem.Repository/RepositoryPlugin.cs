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


        public override bool HasIO => true;

        public override bool IsMainPlugin => true;

        public override string Name => "repository-plugin";

        public string OriginFile
        {
            get
            {
                string file = Path.Combine(PluginPaths.GetPluginConfigDirectory(PluginAssemblyData), "origins.txt");
                return file;
            }
        }

        public void AddOrigin(string origin)
        {
            List<string> origins = ListHelper.LoadList(OriginFile).ToList();
            origins.Add(origin);
            ListHelper.SaveList(OriginFile, origins.Distinct());
        }

        public void RemoveOrigin(string origin)
        {
            List<string> origins = ListHelper.LoadList(OriginFile).ToList();
            origins.Remove(origin);
            ListHelper.SaveList(OriginFile, origins);
        }

        public List<Repository> GetPlugins()
        {
            string[] origins = ListHelper.LoadList(OriginFile);
            List<Repository> repos = new List<Repository>();
            foreach (string origin in origins)
            {
                repos.Add(new Repository(origin, GetOriginData(origin)));
            }

            return repos;
        }

        private List<BasePluginPointer> GetOriginData(string origin)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(origin).Split('\n').Select(x => new BasePluginPointer(x.Trim())).ToList();
            }
        }

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);


            if (!File.Exists(OriginFile)) File.WriteAllText(OriginFile, "");

            PluginManager.LoadPlugins(this);
        }

        public bool IsAllowedPlugin(IPlugin plugin)
        {
            return true;
        }

        public void OnPluginLoad(IPlugin plugin, BasePluginPointer ptr)
        {
        }

        public void OnPluginUnload(IPlugin plugin)
        {
        }

    }
}