using System;
using System.Reflection;

namespace Radio7.ConfigReader.ConfigReaders
{
    public struct MemberInfo
    {
        public string TypeFullName { get; set; }
        public string Name { get; set; }
        public Type MemberType { get; set; }
        public bool IsField { get; set; }
        public FieldInfo FieldInfo { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public string GetFullName()
        {
            return TypeFullName + "." + Name;
        }
    }
}