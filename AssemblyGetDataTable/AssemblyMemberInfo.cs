using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace AssemblyGetDataTable
{
    public class AssemblyMemberInfo
    {
        public string Name { get; }
        public Type Type { get; }
        public IEnumerable<CustomAttributeData> Attributes { get; }
        public string Description { get; }
        public string Summary { get; private set; }
        public AssemblyMemberInfo(MemberInfo memberInfo, XmlDocument Doc)
        {
            _Doc = Doc;
            try
            {

                Type = memberInfo.DeclaringType?.BaseType == typeof(Enum) ?
                    AssemblyDataTable.GetTypeInfo(memberInfo.DeclaringType)?.FirstOrDefault() is FieldInfo info
                        ? info.FieldType : ((FieldInfo)memberInfo).FieldType
                    : ((PropertyInfo)memberInfo).PropertyType;
            }
            catch (InvalidCastException)
            {
                Type = ((FieldInfo)memberInfo).FieldType;
            }
            Name = memberInfo.Name;
            Attributes = memberInfo.CustomAttributes;
            Description = memberInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
            GetXMLDocumentData(memberInfo);
        }
        private XmlDocument _Doc;
        void GetXMLDocumentData(MemberInfo memberInfo)
        {

            if (_Doc is null)
                return;
            string prefix = string.Empty;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    prefix = "E:";
                    break;
                case MemberTypes.Field:
                    prefix = "F:";
                    break;
                case MemberTypes.Method:
                    prefix = "M:";
                    break;
                case MemberTypes.Property:
                    prefix = "P:";
                    break;
                case MemberTypes.TypeInfo:
                    prefix = "T:";
                    break;
            }
            if(string.IsNullOrWhiteSpace(prefix))
                return;
            string path = prefix + memberInfo.DeclaringType?.FullName + "." + memberInfo.Name;

            XmlNode xmlDocuOfMethod = _Doc.SelectSingleNode(
                            "//member[starts-with(@name, '" + path + "')]");
            if (xmlDocuOfMethod is null)
                return;
            foreach (XmlElement element in xmlDocuOfMethod)
            {
                if(element.Name!="summary")
                    continue;
                var cleanStr = Regex.Replace(element.InnerXml, @"\s+", " ");
                if (!string.IsNullOrWhiteSpace(Summary))
                {

                }
                Summary += cleanStr;
            }

            //XmlNode xmlDocuOfMethod = _Doc.SelectSingleNode(
            //    "//member[starts-with(@name, '" + path + "')]");
            //var cleanStr = Regex.Replace(row.InnerXml, @"\s+", " ");
        }

    }
}