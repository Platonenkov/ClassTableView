using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace AssemblyGetDataTable
{
    public class AssemblyInfo
    {
        public Assembly Assembly { get; }
        public IEnumerable<Type> Types { get; }
        public IEnumerable<AssemblyTypeInfo> TypesInfo { get; }

        public AssemblyInfo(string AssemblyName, string AssemblyPath)
        {
            Assembly = System.Reflection.Assembly.Load(AssemblyName);
            Types = AssemblyDataTable.GetPublicTypes(Assembly);

            GetXMLDocumentData(Assembly, AssemblyPath);
            var doc = _Doc;
            TypesInfo = Types.Select(t => new AssemblyTypeInfo(t, doc));

        }
        public AssemblyInfo(Assembly assembly,string AssemblyPath)
        {
            Assembly = assembly;
            Types = AssemblyDataTable.GetPublicTypes(assembly);

            GetXMLDocumentData(Assembly,AssemblyPath);
            var doc = _Doc;

            TypesInfo = Types.Select(t => new AssemblyTypeInfo(t, doc));
        }
        private XmlDocument _Doc;
        void GetXMLDocumentData(Assembly assembly, string AssemblyPath)
        {

            var xml_file_path = Path.ChangeExtension(AssemblyPath, "xml");
            if (!File.Exists(xml_file_path))
                return;

            try
            {
                if (_Doc is null)
                {
                    _Doc = new XmlDocument();
                    _Doc.Load(xml_file_path);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Ошибка загрузки файла документации сбокри:\n{Assembly.CodeBase}\n{e.Message}\n{e}");
            }
        }
    }
}