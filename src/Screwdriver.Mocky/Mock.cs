using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Screwdriver.Mocking.Proxies;

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
            var typeBuilder = ModuleBuilder.DefineType(GetUniqueTypeName<T>(), TypeAttributes.Public | TypeAttributes.Class, typeof(ObjectProxy));
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

            if (!UniqueTypeCounts.ContainsKey(type))
                UniqueTypeCounts.Add(type, 0);

            UniqueTypeCounts[type]++;

            return $"Mock<{typeof(T).Name}_{UniqueTypeCounts[type]}>";
        }

        private static MethodBuilder GetDefaultMethodImplementation<T>(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            var methodBuilder = typeBuilder.DefineMethod($"{nameof(T)}.{methodInfo.Name}",
                MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.HasThis,
                methodInfo.ReturnType, parameters);

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, methodInfo.Name);

            var objArray = il.DeclareLocal(typeof(object[]));
            il.Emit(OpCodes.Ldc_I4, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc, objArray.LocalIndex);

            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldloc, objArray.LocalIndex);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldarg, i + 1);

                if (parameters[i].IsValueType)
                    il.Emit(OpCodes.Box, parameters[i]);

                il.Emit(OpCodes.Stelem, typeof(object));
            }

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Call, typeof(ObjectProxy).GetMethod("CallMethod"));
            il.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        private static readonly IDictionary<Type, int> UniqueTypeCounts = new Dictionary<Type, int>();
        private static readonly ModuleBuilder ModuleBuilder;
    }
}