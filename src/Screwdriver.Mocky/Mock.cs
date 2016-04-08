using System;
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
            var typeBuilder = ModuleBuilder.DefineType($"Mock<{typeof(T).Name}>", TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeof(T));

            foreach (var methodInfo in typeof(T).GetMethods())
            {
                var methodImplementation = GetDefaultMethodImplementation<T>(typeBuilder, methodInfo);
                typeBuilder.DefineMethodOverride(methodImplementation, methodInfo);
            }

            var type = typeBuilder.CreateType();

            return (T)Activator.CreateInstance(type);
        }

        private static MethodBuilder GetDefaultMethodImplementation<T>(TypeBuilder typeBuilder, MethodInfo methodInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod($"{nameof(T)}.{methodInfo.Name}",
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final,
                CallingConventions.HasThis,
                methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray()
                );

            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ret);

            return methodBuilder;
        }

        private static readonly ModuleBuilder ModuleBuilder;
    }
}