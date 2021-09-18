using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyGetDataTable
{
    public class AssemblyInfoModel
    {
        public Assembly Assembly { get; }
        public IEnumerable<Type> Types { get; }
        public IEnumerable<AssemblyTypeInfoModel> TypesInfo { get; }

        public AssemblyInfoModel(string AssemblyName)
        {
            Assembly = System.Reflection.Assembly.Load(AssemblyName);
            Types = AssemblyDataTable.GetPublicTypes(Assembly);
            TypesInfo = Types.Select(t => new AssemblyTypeInfoModel(t));

        }
        public AssemblyInfoModel(Assembly assembly)
        {
            Assembly = assembly;
            Types = AssemblyDataTable.GetPublicTypes(assembly);
            TypesInfo = Types.Select(t => new AssemblyTypeInfoModel(t));
        }
    }
}