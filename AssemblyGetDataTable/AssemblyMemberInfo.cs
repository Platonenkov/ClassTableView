using System;
using System.Collections;
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
        public string Note { get; set; }
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

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                //case MemberTypes.Field:
                    if (Nullable.GetUnderlyingType(Type)!=null)
                    {
                        Type = Type.GetGenericArguments()[0];
                        Note += "CanBeNull ";
                    }
                    if (((PropertyInfo)memberInfo).GetMethod.IsVirtual)
                    {
                        string info = null;
                        var type = FindIEnumerable(Type);
                        if (type is not null)
                            info = type.GetGenericArguments()[0].Name;
                        info ??= Type.Name;
                        if (type is not null && !type.GetGenericArguments()[0].IsValueType && !(type.GetGenericArguments()[0] == typeof(string)))
                            Note += $"Foreign Key: {info}";
                        else
                            Note += Type.IsValueType
                                ? memberInfo.Name.ToUpper().Contains("ID") ? "Primary Key" : null
                                : $"Foreign Key: {info}";

                        //if ((type is not null && !type.GetGenericArguments()[0].IsValueType&&!(type.GetGenericArguments()[0] == typeof(string))) || Type.IsValueType)
                        //    Note = Type.IsValueType 
                        //        ? memberInfo.Name.ToUpper().Contains("ID") ? "Primary Key" : null 
                        //        : $"Foreign Key: {info}";
                    }
                    break;
            }

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
            if (string.IsNullOrWhiteSpace(prefix))
                return;
            Summary = GetSummary(memberInfo, prefix);
        }
        private string GetSummary(MemberInfo memberInfo, string prefix)
        {
            string path = prefix + memberInfo.DeclaringType?.FullName + "." + memberInfo.Name;

            XmlNode xmlDocuOfMethod = _Doc.SelectSingleNode(
                "//member[starts-with(@name, '" + path + "')]");
            if (xmlDocuOfMethod is null)
                return string.Empty;
            var summary = string.Empty;

            foreach (XmlElement element in xmlDocuOfMethod)
            {
                if (element.Name == "inheritdoc")
                {
                    var types = memberInfo.ReflectedType.GetInterfaces();
                    foreach (var type1 in types)
                    {
                        var member = type1.GetMember(memberInfo.Name).FirstOrDefault();
                        if (member is null)
                            continue;

                        summary += $"\n{GetSummary(member, prefix)}".Trim();
                    }
                    break;

                }
                var cleanStr = Regex.Replace(element.InnerXml, @"\s+", " ");
                summary += $"\n{cleanStr}".Trim();
            }

            return summary;
        }
        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}