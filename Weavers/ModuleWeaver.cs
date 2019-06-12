using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Weavers
{
    public class ModuleWeaver : BaseModuleWeaver
    {
#if NATIVEDEP
        private TypeDefinition nativeObjectType;
        private TypeDefinition nativeHandleType;
        private MethodDefinition nativeObjectFromPointer;
        private MethodDefinition nativeObject_NativeHandle_Getter;
        private MethodDefinition nativeHandle_Address_Getter;
        private int num;
        public override void AfterWeaving()
        {
            base.AfterWeaving();
        }
        public override void Execute()
        {
            var methodInfoType = ModuleDefinition.ImportReference(typeof(MethodInfo));
            var getMethodFromHandleMethod = typeof(MethodInfo).GetMethod("GetMethodFromHandle", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(RuntimeMethodHandle) }, null);
            var getMethodFromHandle = ModuleDefinition.ImportReference(getMethodFromHandleMethod);
            var prepareMethodMethod = typeof(RuntimeHelpers).GetMethod("PrepareMethod", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(RuntimeMethodHandle) }, null);
            var prepareMethod = ModuleDefinition.ImportReference(prepareMethodMethod);
            var nativeWeaveType = ModuleDefinition.Types.FirstOrDefault(t => t.FullName == "Open3dmm.NativeWeaver");

            nativeHandleType = ModuleDefinition.Types.FirstOrDefault(t => t.FullName == "Open3dmm.NativeHandle");
            nativeHandle_Address_Getter = nativeHandleType.Properties.FirstOrDefault(p => p.Name == "Address").GetMethod;

            nativeObjectType = ModuleDefinition.Types.FirstOrDefault(t => t.FullName == "Open3dmm.NativeObject");
            nativeObject_NativeHandle_Getter = nativeObjectType.Properties.FirstOrDefault(p => p.Name == "NativeHandle").GetMethod;
            nativeObjectFromPointer = nativeObjectType.Methods.FirstOrDefault(mt => mt.Name == "FromPointer");
            var init = nativeWeaveType.Methods.FirstOrDefault(mt => mt.Name == "Init");
            var hookType = ModuleDefinition.Types.FirstOrDefault(t => t.FullName == "Open3dmm.Hook");
            var createHook = hookType.Methods.FirstOrDefault(mt => mt.Name == "CreateHook");
            init.Body.InitLocals = true;
            init.Body.Variables.Clear();
            init.Body.Instructions.Clear();
            var ilInit = init.Body.GetILProcessor();
            var targetMethods = new List<IGrouping<(MethodDefinition, CallingConvention), CustomAttribute>>();
            var targetProperties = new List<(PropertyDefinition prop, CustomAttribute attr)>();
            foreach (var type in this.ModuleDefinition.Types)
            {
                targetMethods.AddRange(from method in type.Methods
                                       from attr in method.CustomAttributes.Where(a => a.AttributeType.FullName == "Open3dmm.HookFunctionAttribute")
                                       let propCallingConvention = attr.Properties.First(p => p.Name.Contains("CallingConvention"))
                                       let callingConvention = (CallingConvention)propCallingConvention.Argument.Value
                                       group attr by (method, callingConvention));

                foreach (var prop in type.Properties)
                {
                    var attr = prop.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "Open3dmm.NativeFieldOffsetAttribute");
                    if (attr == null)
                        continue;

                    targetProperties.Add((prop, attr));
                }
            }

            if (targetMethods.Count > 0)
            {
                TypeDefinition declaringType;
                foreach (var grouping in targetMethods)
                {
                    var (method, callingConvention) = grouping.Key;

                    if (method.HasGenericParameters)
                        throw new NotSupportedException("Methods with generic parameters not supported");

                    var transitionalMethod = CreateTransitionalMethod(callingConvention, method, out var delegateType);
                    declaringType = method.DeclaringType;
                    declaringType.Methods.Add(transitionalMethod.Resolve());
                    transitionalMethod = ModuleDefinition.ImportReference(transitionalMethod);

                    foreach (var attr in grouping)
                    {
                        var hookAddress = (IntPtr)Convert.ToInt32(attr.ConstructorArguments[0].Value);

                        if (transitionalMethod != null)
                        {
                            ilInit.Emit(OpCodes.Ldc_I4, (int)hookAddress);
                            //il.Emit(OpCodes.Ldtoken, transitionalMethod);
                            //il.Emit(OpCodes.Call, prepareMethod);
                            ilInit.Emit(OpCodes.Ldtoken, transitionalMethod);
                            ilInit.Emit(OpCodes.Call, getMethodFromHandle);
                            ilInit.Emit(OpCodes.Castclass, methodInfoType);

                            var createHookGeneric = createHook.MakeGenericMethod(delegateType);
                            ilInit.Emit(OpCodes.Call, createHookGeneric);
                        }
                    }
                }
            }

            ilInit.Emit(OpCodes.Ret);

            // NativeFieldOffset Properties
            //var unsafeAsMethod = typeof(Unsafe)
            //             .GetMethods()
            //             .Where(m => m.Name == "As")
            //             .Select(m => new
            //             {
            //                 Method = m,
            //                 Params = m.GetParameters(),
            //                 Args = m.GetGenericArguments()
            //             })
            //             .Where(x => x.Params.Length == 1
            //                         && x.Args.Length == 2
            //                         && x.Params[0].ParameterType == x.Args[0])
            //             .Select(x => x.Method)
            //             .Single();

            var intPtrToPointerMethod = typeof(IntPtr)
                         .GetMethods()
                         .Where(m => m.Name == "op_Explicit")
                         .Select(m => new
                         {
                             Method = m,
                             Params = m.GetParameters()
                         })
                         .Where(x => x.Params.Length == 1
                                     && x.Method.ReturnType == typeof(void*))
                         .Select(x => x.Method)
                         .Single();

            var intPtrAddMethod = typeof(IntPtr).GetMethod("Add", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(IntPtr), typeof(int) }, null);
            var typeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(RuntimeTypeHandle) }, null);

            //var unsafeAs = ModuleDefinition.ImportReference(unsafeAsMethod);
            var intPtrAdd = ModuleDefinition.ImportReference(intPtrAddMethod);
            var typeFromHandle = ModuleDefinition.ImportReference(typeFromHandleMethod);
            var intPtrToPointer = ModuleDefinition.ImportReference(intPtrToPointerMethod);

            foreach (var (prop, attr) in targetProperties)
            {
                var fieldOffset = Convert.ToInt32(attr.ConstructorArguments[0].Value);
                var ilGetter = prop.GetMethod.Body.GetILProcessor();
                ilGetter.Body.Instructions.Clear();
                ilGetter.Emit(OpCodes.Ldarg_0); // this
                ilGetter.Emit(OpCodes.Call, nativeObject_NativeHandle_Getter); // Get Handle
                ilGetter.Emit(OpCodes.Call, nativeHandle_Address_Getter); // Get address
                ilGetter.Emit(OpCodes.Ldc_I4, fieldOffset);
                ilGetter.Emit(OpCodes.Call, intPtrAdd);
                ilGetter.Emit(OpCodes.Call, intPtrToPointer);
                ilGetter.Emit(OpCodes.Ret);
            }
        }

        private TypeReference GetDelegateType(CallingConvention callingConvention, int count)
        {
            switch (callingConvention)
            {
                case CallingConvention.ThisCall:
                    return ModuleDefinition.GetType("Open3dmm.ThisCall" + count);
                case CallingConvention.StdCall:
                    return ModuleDefinition.GetType("Open3dmm.StdCall" + count);
                case CallingConvention.Cdecl:
                    return ModuleDefinition.GetType("Open3dmm.Cdecl" + count);
            }
            throw new NotSupportedException();
        }

        private MethodReference CreateTransitionalMethod(CallingConvention callingConvention, MethodDefinition method, out TypeReference delegateType)
        {
            string methodName = $"___Transitional_{callingConvention}_Method{num++}_{method.Name}";
            var methodAttr = method.Attributes;
            methodAttr &= ~Mono.Cecil.MethodAttributes.Public;
            methodAttr &= ~Mono.Cecil.MethodAttributes.Virtual;
            methodAttr &= ~Mono.Cecil.MethodAttributes.Abstract;
            methodAttr &= ~Mono.Cecil.MethodAttributes.SpecialName;
            methodAttr &= ~Mono.Cecil.MethodAttributes.RTSpecialName;
            methodAttr |= Mono.Cecil.MethodAttributes.Static;
            var declaringType = method.DeclaringType;
            var intPtrType = ModuleDefinition.TypeSystem.IntPtr;
            var transitionalMethod = new MethodDefinition(methodName, methodAttr, intPtrType);
            transitionalMethod.Body.InitLocals = true;
            transitionalMethod.Body.Variables.Clear();
            var il = transitionalMethod.Body.GetILProcessor();
            ParameterDefinition returnParam = default;
            short arg = 0;
            bool staticThisCall = false;

            switch (callingConvention)
            {
                case CallingConvention.ThisCall:
                    if (!method.IsStatic && method.HasThis)
                    {
                        // Add 'this' parameter
                        transitionalMethod.Parameters.Add(new ParameterDefinition(intPtrType) { Name = "self" });
                        if (declaringType.IsValueType)
                        {
                            il.Emit(OpCodes.Ldarg_0);
                        }
                        else
                        {
                            if (!nativeObjectType.IsAssignableFrom(declaringType))
                            {
                                LogError("Declaring type must be Struct or NativeObject type");
                                goto Error;
                            }
                            var nativeObjectFromPointerGeneric = nativeObjectFromPointer.MakeGenericMethod(declaringType);
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Call, nativeObjectFromPointerGeneric);
                        }
                        arg++;
                    }
                    else staticThisCall = true;
                    goto case CallingConvention.StdCall;

                case CallingConvention.Cdecl:
                case CallingConvention.StdCall:
                    foreach (var param in method.Parameters)
                    {
                        var newParam = new ParameterDefinition(intPtrType) { Name = param.Name };
                        foreach (var paramAttr in param.CustomAttributes)
                            newParam.CustomAttributes.Add(paramAttr);
                        transitionalMethod.Parameters.Add(newParam);
                        if (param.IsReturnValue)
                            returnParam = param;
                        else
                        {
                            if (!param.IsOut && !param.IsIn && !param.ParameterType.IsByReference && !param.ParameterType.IsPointer && param.ParameterType != ModuleDefinition.TypeSystem.IntPtr && param.ParameterType != ModuleDefinition.TypeSystem.UIntPtr)
                            {
                                // Pass by value.
                                if (param.ParameterType.IsValueType || param.ParameterType.IsPointer)
                                {
                                    // Value Type
                                    il.Emit(OpCodes.Ldarg, arg++);
                                    //if (param.ParameterType == ModuleDefinition.TypeSystem.Int32
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.UInt32
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.Int16
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.UInt16
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.SByte
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.Byte
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.Char
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.Boolean
                                    // || param.ParameterType == ModuleDefinition.TypeSystem.Single
                                    // || param.ParameterType.Resolve().IsEnum)
                                    //    il.Emit(OpCodes.Ldarg, arg++);
                                    //else
                                    //{
                                    //    transitionalMethod.Body.Variables.Add(new VariableDefinition(param.ParameterType));
                                    //    il.Emit(OpCodes.Ldloca, loc);
                                    //    il.Emit(OpCodes.Ldarg, arg++);
                                    //    il.Emit(OpCodes.Ldobj, param.ParameterType);
                                    //    il.Emit(OpCodes.Stobj, param.ParameterType);
                                    //    il.Emit(OpCodes.Ldloc, loc++);
                                    //}
                                }
                                else
                                {
                                    // Reference Type
                                    if (!nativeObjectType.IsAssignableFrom(param.ParameterType))
                                    {
                                        LogError("Unsupported reference type parameter");
                                        goto Error;
                                    }

                                    var nativeObjectFromPointerGeneric = nativeObjectFromPointer.MakeGenericMethod(param.ParameterType);
                                    il.Emit(OpCodes.Ldarg, arg++);
                                    il.Emit(OpCodes.Call, nativeObjectFromPointerGeneric);
                                }
                            }
                            else
                            {
                                // Pointer
                                il.Emit(OpCodes.Ldarg, arg++);
                            }
                        }
                    }
                    break;
            }
            il.Emit(OpCodes.Call, method);
            if (method.ReturnType == ModuleDefinition.TypeSystem.Void)
            {
                if (method.IsConstructor)
                    il.Emit(OpCodes.Ldarg_0);
                else
                    il.Emit(OpCodes.Ldc_I4_0);

            }
            else if (nativeObjectType.IsAssignableFrom(method.ReturnType))
            {
                // Reference Return Type
                var nop = il.Create(OpCodes.Nop);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brtrue_S, nop);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_0); // return 0
                il.Emit(OpCodes.Ret);
                il.Append(nop);
                il.Emit(OpCodes.Call, nativeObject_NativeHandle_Getter); // Get Handle
                il.Emit(OpCodes.Call, nativeHandle_Address_Getter); // Get address
            }
            il.Emit(OpCodes.Ret);
            if (staticThisCall)
                delegateType = GetDelegateType(callingConvention, method.Parameters.Count - 1);
            else
                delegateType = GetDelegateType(callingConvention, method.Parameters.Count);
            return transitionalMethod;
        Error:
            delegateType = default;
            return default;
        }
#else
        public override void Execute()
        {
        }
#endif
        public override IEnumerable<string> GetAssembliesForScanning()
        {
            return Enumerable.Empty<string>();
        }
    }
}
