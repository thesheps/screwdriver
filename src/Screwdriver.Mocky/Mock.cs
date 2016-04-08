using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Screwdriver.Mocking
{
    public class Mock
    {
        static Mock()
        {
            var assemblyName = new AssemblyName { Name = "Mocks" };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
        }

        public static T Out<T>()
        {
            var typeBuilder = ModuleBuilder.DefineType(GetUniqueTypeName<T>(), TypeAttributes.Public | TypeAttributes.Class, typeof(Proxy));
            typeBuilder.AddInterfaceImplementation(typeof(T));

            foreach (var methodInfo in typeof(T).GetMethods())
            {
                var methodImplementation = GetDefaultMethodImplementation<T>(typeBuilder, methodInfo);
                typeBuilder.DefineMethodOverride(methodImplementation, methodInfo);
            }

            var type = typeBuilder.CreateType();

            return (T)Activator.CreateInstance(type);
        }

        private static string GetUniqueTypeName<T>()
        {
            var type = typeof(T);

            if (!_uniqueTypeCounts.ContainsKey(type))
                _uniqueTypeCounts.Add(type, 0);

            _uniqueTypeCounts[type]++;

            return $"Mock<{typeof(T).Name}_{_uniqueTypeCounts[type]}>";
        }

        private static MethodBuilder GetDefaultMethodImplementation<T>(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod($"{nameof(T)}.{methodInfo.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis,
                methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, methodInfo.Name);
            il.Emit(OpCodes.Call, typeof(Proxy).GetMethod("CallMethod"));
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        private static IDictionary<Type, int> _uniqueTypeCounts = new Dictionary<Type, int>();
        private static readonly ModuleBuilder ModuleBuilder;
    }
}