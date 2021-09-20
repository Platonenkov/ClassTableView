using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AssemblyGetDataTable
{
    public static class AssemblyDataTable
    {
        public static IEnumerable<Type>  GetPublicTypes(string AssenblyName) => 
            Assembly.Load(AssenblyName).GetTypes().Where(t => t.IsPublic && t.IsClass || t.IsEnum);
        public static IEnumerable<Type>  GetPublicTypes(Assembly assenbly) =>
            assenbly.GetTypes().Where(t => t.IsPublic && t.IsClass || t.IsEnum);

        public static IEnumerable<MemberInfo> GetTypeInfo(Type type)
        {
            return type.BaseType == typeof(Enum)
                ? type.GetMembers(
                //BindingFlags.Instance
                //| BindingFlags.Static
                //| BindingFlags.Public
                //| BindingFlags.NonPublic
                //| BindingFlags.DeclaredOnly

                ).Where(v => v.MemberType == MemberTypes.Field)
                : type.GetMembers()
                   .Where(v => v.MemberType == MemberTypes.Property);
        }

        public static (Assembly assembly, IEnumerable<Type> Types)? LoadAssemblyFromFileInfo(FileInfo file)=>LoadAssemblyFromFile(file.FullName);

        public static (Assembly assembly, IEnumerable<Type> Types)? LoadAssemblyFromFile(string FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                return null;
            var assembly = Assembly.LoadFrom(FilePath);
            var types = GetPublicTypes(assembly);

            return (assembly, types);
        }

        public static IEnumerable<AssemblyInfo> GetAssemblyInfo(Assembly assembly, string AssemblyPath)
        {
            var assemblies = new List<Assembly> { assembly };
            var entry = assembly.GetReferencedAssemblies().Select(a => Assembly.Load(a.Name)).ToList();
            if (entry?.Count > 0)
                assemblies.AddRange(entry);

            return assemblies.Select(a=> new AssemblyInfo(a,Path.Combine(AssemblyPath,a.ManifestModule.Name)));
        }
    }
}
