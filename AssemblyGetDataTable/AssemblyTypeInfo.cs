using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace AssemblyGetDataTable
{
    public class AssemblyTypeInfo
    {
        private readonly XmlDocument _Doc;
        public  Type Type { get; }
        public  IEnumerable<MemberInfo> Members { get; }
        public  IEnumerable<AssemblyMemberInfo> MembersInfo { get; }
        public string Summary { get; private set; }

        public AssemblyTypeInfo(Type type, XmlDocument Doc)
        {
            _Doc = Doc;
            Type = type;
            Members = AssemblyDataTable.GetTypeInfo(Type);
            GetXMLDocumentData(type);
            MembersInfo = Members.Select(m => new AssemblyMemberInfo(m, Doc));
        }
        void GetXMLDocumentData(Type type)
        {

            if (_Doc is null)
                return;
            string path = "T:" + type.FullName;

            XmlNode xmlDocuOfMethod = _Doc.SelectSingleNode(
                "//member[starts-with(@name, '" + path + "')]");
            if(xmlDocuOfMethod is null)
                return;
            foreach (XmlElement element in xmlDocuOfMethod)
            {
                var cleanStr = Regex.Replace(element.InnerXml, @"\s+", " ");
                Summary += cleanStr;
            }
        }

    }
}