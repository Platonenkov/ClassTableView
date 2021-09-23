using System.Reflection;
using System.Runtime.Loader;

namespace ClassTableView
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public CustomAssemblyLoadContext() : base(isCollectible: true) { }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}