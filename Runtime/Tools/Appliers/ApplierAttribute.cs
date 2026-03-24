using System;

namespace EC.Appliers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ApplierAttribute : Attribute
    {
        public Type TargetType { get; }
        public Type ValueType { get; }
        public string TargetName { get; }
        public string ValueName { get; }

        public ApplierAttribute(Type targetType, Type valueType)
        {
            TargetType = targetType;
            ValueType = valueType;
            TargetName = targetType.Name;
            ValueName = valueType.Name;
        }
        public ApplierAttribute(Type targetType, Type valueType, string targetName, string valueName)
        {
            TargetType = targetType;
            ValueType = valueType;
            TargetName = targetName;
            ValueName = valueName;
        }
    }
}