using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AssemblyGetDataTable
{
    public class AssemblyMemberInfoModel
    {
        public  string Name { get; }
        public  Type Type{ get; }
        public  IEnumerable<CustomAttributeData> Attributes{ get; }
        public  string Description{ get; }
        public  string Summary{ get; }
        public AssemblyMemberInfoModel(MemberInfo memberInfo)
        {
            try
            {

                Type = memberInfo.DeclaringType?.BaseType == typeof(Enum) ?
                    AssemblyDataTable.GetTypeInfo(memberInfo.DeclaringType)?.FirstOrDefault() is FieldInfo info 
                        ? info.FieldType : ((FieldInfo)memberInfo).FieldType 
                    : ((PropertyInfo)memberInfo).PropertyType;
            }
            catch (InvalidCastException e)
            {
                Type = ((FieldInfo)memberInfo).FieldType;
            }
            Name = memberInfo.Name;
            Attributes = memberInfo.CustomAttributes; 
            Description = memberInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }
    }
}