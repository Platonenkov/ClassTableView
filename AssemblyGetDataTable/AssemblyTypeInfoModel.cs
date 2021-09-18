using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyGetDataTable
{
    public class AssemblyTypeInfoModel
    {
        public  Type Type { get; }
        public bool IsSelected { get; set; }
        public  IEnumerable<MemberInfo> Members { get; }
        public  IEnumerable<AssemblyMemberInfoModel> MembersInfo { get; }

        public AssemblyTypeInfoModel(Type type)
        {
            Type = type;
            Members = AssemblyDataTable.GetTypeInfo(Type);
            MembersInfo = Members.Select(m => new AssemblyMemberInfoModel(m));
        }
    }
}