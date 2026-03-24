using System;
using System.Collections.Generic;
using System.Linq;

namespace EC
{
    public static class PolymorphicTypeCache
    {
        private static readonly Dictionary<Type, Type[]> Cache = new();
        private static readonly Dictionary<Type, Type[]> GenericInterfaceCache = new();

        public static Type[] GetTypes(Type baseType)
        {
            if (Cache.TryGetValue(baseType, out var types))
                return types;
            types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => baseType.IsAssignableFrom(t) &&
                            !t.IsInterface &&
                            !t.IsAbstract &&
                            !t.IsGenericType)
                .ToArray();
            Cache[baseType] = types;
            return types;
        }
        public static Type[] GetGenericImplementations(Type genericInterface)
        {
            if (!genericInterface.IsGenericTypeDefinition)
                throw new ArgumentException("genericInterface will be generic type definition, example typeof(IVariableTarget<>)");
            if (GenericInterfaceCache.TryGetValue(genericInterface, out var types))
                return types;
            types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .Where(t => t.GetInterfaces()
                             .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface))
                .ToArray();
            GenericInterfaceCache[genericInterface] = types;
            return types;
        }
        public static Type[] GetGenericArgumentsForInterface(Type genericInterface)
        {
            var implementations = GetGenericImplementations(genericInterface);
            var genericArgs = implementations
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
                    .Select(i => i.GetGenericArguments()[0])
                )
                .Distinct()
                .ToArray();
            return genericArgs;
        }
    }
}