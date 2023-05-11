#if !UNITY_EDITOR
using Aki.Reflection.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GamePanelHUDCore.Utils
{
    //1.1.0
    public static class RefHelp
    {
        public static Func<T, F> ObjectFieldGetAccess<T, F>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var delegateInstanceType = typeof(T);
            var declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            bool isObject = typeof(F) == typeof(object);
            bool needBox;

            if (isObject && fieldInfo.FieldType.IsValueType)
            {
                needBox = true;
            }
            else
            {
                needBox = false;
            }

            var dmd = new DynamicMethod($"__get_{delegateInstanceType.Name}_fi_{fieldInfo.Name}", typeof(F), new[] { delegateInstanceType });

            var ilGen = dmd.GetILGenerator();
            if (fieldInfo.IsStatic)
            {
                ilGen.Emit(OpCodes.Ldsfld, fieldInfo);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                if (typeof(T) == typeof(object))
                {
                    ilGen.Emit(OpCodes.Castclass, declaringType);
                }

                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
            }

            if (needBox)
            {
                ilGen.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else if (isObject)
            {
                ilGen.Emit(OpCodes.Castclass, typeof(F));
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T, F>)dmd.CreateDelegate(typeof(Func<T, F>));
        }

        public static Action<T, F> ObjectFieldSetAccess<T, F>(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            var delegateInstanceType = typeof(T);
            var declaringType = fieldInfo.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            bool isObject = typeof(F) == typeof(object);
            bool needBox;

            if (isObject && fieldInfo.FieldType.IsValueType)
            {
                needBox = true;
            }
            else
            {
                needBox = false;
            }

            var dmd = new DynamicMethod($"__set_{delegateInstanceType.Name}_fi_{fieldInfo.Name}", null, new[] { delegateInstanceType, typeof(F) });

            var ilGen = dmd.GetILGenerator();
            if (fieldInfo.IsStatic)
            {
                ilGen.Emit(OpCodes.Ldarg_1);

                if (needBox)
                {
                    ilGen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                }
                else if (isObject)
                {
                    ilGen.Emit(OpCodes.Castclass, typeof(F));
                }

                ilGen.Emit(OpCodes.Stsfld, fieldInfo);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                if (typeof(T) == typeof(object))
                {
                    ilGen.Emit(OpCodes.Castclass, declaringType);
                }

                ilGen.Emit(OpCodes.Ldarg_1);

                if (needBox)
                {
                    ilGen.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                }
                else if (isObject)
                {
                    ilGen.Emit(OpCodes.Castclass, typeof(F));
                }

                ilGen.Emit(OpCodes.Stfld, fieldInfo);
            }
            ilGen.Emit(OpCodes.Ret);

            return (Action<T, F>)dmd.CreateDelegate(typeof(Action<T, F>));
        }

        public static DelegateType ObjectMethodDelegate<DelegateType>(MethodInfo method, bool virtualCall = true) where DelegateType : Delegate
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var delegateType = typeof(DelegateType);

            var declaringType = method.DeclaringType;
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            var delegateMethod = delegateType.GetMethod("Invoke");
            if (delegateMethod == null)
            {
                throw new ArgumentNullException(nameof(delegateMethod));
            }
            var delegateParameters = delegateMethod.GetParameters();
            var delegateParameterTypes = delegateParameters.Select(x => x.ParameterType).ToArray();

            Type returnType;
            bool needBox;

            if (delegateMethod.ReturnType == typeof(object) && method.ReturnType.IsValueType)
            {
                returnType = typeof(object);

                needBox = true;
            }
            else
            {
                returnType = method.ReturnType;

                needBox = false;
            }

            var dmd = new DynamicMethod("OpenInstanceDelegate_" + method.Name, returnType, delegateParameterTypes);

            var ilGen = dmd.GetILGenerator();

            Type[] parameterTypes;
            int num;

            if (!method.IsStatic)
            {
                var parameters = method.GetParameters();
                var numParameters = parameters.Length;
                parameterTypes = new Type[numParameters + 1];
                parameterTypes[0] = typeof(object);

                for (int i = 0; i < numParameters; i++)
                {
                    parameterTypes[i + 1] = parameters[i].ParameterType;
                }

                if (declaringType.IsValueType)
                {
                    ilGen.Emit(OpCodes.Ldarga_S, 0);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                }

                ilGen.Emit(OpCodes.Castclass, declaringType);

                num = 1;
            }
            else
            {
                parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();

                num = 0;
            }

            for (int i = num; i < parameterTypes.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg, i);

                Type parameterType = parameterTypes[i];

                bool isValueType = parameterType.IsValueType;

                if (!isValueType)
                {
                    ilGen.Emit(OpCodes.Castclass, parameterType);
                }
                //DelegateParameterTypes i == parameterTypes i
                else if (delegateParameterTypes[i] == typeof(object))
                {
                    ilGen.Emit(OpCodes.Unbox_Any, parameterType);
                }
            }

            if (method.IsStatic || !virtualCall)
            {
                ilGen.Emit(OpCodes.Call, method);
            }
            else
            {
                ilGen.Emit(OpCodes.Callvirt, method);
            }

            if (needBox)
            {
                ilGen.Emit(OpCodes.Box, method.ReturnType);
            }

            ilGen.Emit(OpCodes.Ret);

            return (DelegateType)dmd.CreateDelegate(delegateType);
        }

        public class PropertyRef<T, F> where T : class
        {
            private Func<T, F> RefGetValue;

            private Action<T, F> RefSetValue;

            private PropertyInfo PropertyInfo;

            private MethodInfo GetMethodInfo;

            private MethodInfo SetMethodInfo;

            private Type TType;

            private T Instance;

            public Type InType => TType;

            public Type PropertyType => PropertyInfo.PropertyType;

            public PropertyRef(PropertyInfo propertyInfo, object instance)
            {
                if (propertyInfo == null)
                {
                    throw new Exception("PropertyInfo is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string propertyName, bool declaredOnly, object instance)
            {
                BindingFlags flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                PropertyInfo propertyInfo = type.GetProperty(propertyName, flags);
                
                if (propertyInfo == null)
                {
                    throw new Exception(propertyName + " is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string[] propertyNames, bool declaredOnly, object instance)
            {
                BindingFlags flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                PropertyInfo propertyInfo = propertyNames.Select(x => type.GetProperty(x, flags)).FirstOrDefault(x => x != null);

                if (propertyInfo == null)
                {
                    throw new Exception(propertyNames.First() + " is null");
                }

                Init(propertyInfo, instance);
            }

            private void Init(PropertyInfo propertyInfo, object instance)
            {
                PropertyInfo = propertyInfo;

                TType = PropertyInfo.DeclaringType;

                Instance = (T)instance;

                bool isObject = typeof(T) == typeof(object);

                if (PropertyInfo.CanRead)
                {
                    GetMethodInfo = PropertyInfo.GetGetMethod(true);

                    if (isObject)
                    {
                        RefGetValue = ObjectMethodDelegate<Func<T, F>>(GetMethodInfo);
                    }
                    else
                    {
                        RefGetValue = AccessTools.MethodDelegate<Func<T, F>>(GetMethodInfo);    
                    }     
                }

                if (PropertyInfo.CanWrite)
                {
                    SetMethodInfo = PropertyInfo.GetSetMethod(true);

                    if (isObject)
                    {
                        RefSetValue = ObjectMethodDelegate<Action<T, F>>(SetMethodInfo);
                    }
                    else
                    {
                        RefSetValue = AccessTools.MethodDelegate<Action<T, F>>(SetMethodInfo);
                    }     
                }
            }

            public static PropertyRef<T, F> Create(PropertyInfo propertyInfo, object instance)
            {
                return new PropertyRef<T, F>(propertyInfo, instance);
            }

            public static PropertyRef<T, F> Create(string propertyName, bool declaredOnly = false, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertyName, declaredOnly, instance);
            }

            public static PropertyRef<T, F> Create(string[] propertyNames, bool declaredOnly = false, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertyNames, declaredOnly, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string propertyName, bool declaredOnly = false, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertyName, declaredOnly, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string[] propertyNames, bool declaredOnly = false, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertyNames, declaredOnly, instance);
            }

            public F GetValue(T instance)
            {
                if (RefGetValue == null)
                {
                    throw new ArgumentNullException(nameof(RefGetValue));
                }

                if (instance != null && TType.IsInstanceOfType(instance))
                {
                    return RefGetValue(instance);
                }
                else if (Instance != null && instance == null)
                {
                    return RefGetValue(Instance);
                }
                else
                {
                    return default;
                }
            }

            public void SetValue(T instance, F value)
            {
                if (RefSetValue == null)
                {
                    throw new ArgumentNullException(nameof(RefSetValue));
                }

                if (instance != null && TType.IsInstanceOfType(instance))
                {
                    RefSetValue(instance, value);
                }
                else if (Instance != null && instance == null)
                {
                    RefSetValue(Instance, value);
                }
            }
        }

        public class FieldRef<T, F> where T : class
        {
            private AccessTools.FieldRef<T, F> HarmonyFieldRef;

            private Func<T, F> RefGetValue;

            private Action<T, F> RefSetValue;

            private FieldInfo FieldInfo;

            private Type TType;

            private T Instance;

            private bool UseHarmony;

            public Type InType => TType;

            public Type FieldType => FieldInfo.FieldType;

            public FieldRef(FieldInfo fieldInfo, object instance)
            {
                if (fieldInfo == null)
                {
                    throw new Exception("FieldInfo is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string fieldName, bool declaredOnly, object instance)
            {
                BindingFlags flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                FieldInfo fieldInfo = type.GetField(fieldName, flags);

                if (fieldInfo == null)
                {
                    throw new Exception(fieldName + " is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string[] fieldNames, bool declaredOnly, object instance)
            {
                BindingFlags flags = declaredOnly ? AccessTools.allDeclared : AccessTools.all;

                FieldInfo fieldInfo = fieldNames.Select(x => type.GetField(x, flags)).FirstOrDefault(x => x != null);

                if (fieldInfo == null)
                {
                    throw new Exception(fieldNames.First() + " is null");
                }

                Init(fieldInfo, instance);
            }

            public static FieldRef<T, F> Create(FieldInfo fieldInfo, object instance = null)
            {
                return new FieldRef<T, F>(fieldInfo, instance);
            }

            public static FieldRef<T, F> Create(string fieldName, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldName, declaredOnly, instance);
            }

            public static FieldRef<T, F> Create(string[] fieldNames, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldNames, declaredOnly, instance);
            }

            public static FieldRef<T, F> Create(Type type, string fieldName, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldName, declaredOnly, instance);
            }

            public static FieldRef<T, F> Create(Type type, string[] fieldNames, bool declaredOnly = false, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldNames, declaredOnly, instance);
            }

            private void Init(FieldInfo fieldInfo, object instance = null)
            {
                FieldInfo = fieldInfo;

                TType = FieldInfo.DeclaringType;

                Instance = (T)instance;

                if (typeof(F) == typeof(object))
                {
                    RefGetValue = ObjectFieldGetAccess<T, F>(FieldInfo);
                    RefSetValue = ObjectFieldSetAccess<T, F>(FieldInfo);    
                    UseHarmony = false;
                }
                else
                {
                    HarmonyFieldRef = AccessTools.FieldRefAccess<T, F>(FieldInfo);
                    UseHarmony = true;
                }
            }

            public F GetValue(T instance)
            {
                if (UseHarmony)
                {
                    if (HarmonyFieldRef == null)
                    {
                        throw new ArgumentNullException(nameof(HarmonyFieldRef));
                    }

                    if (instance != null && TType.IsInstanceOfType(instance))
                    {
                        return HarmonyFieldRef(instance);
                    }
                    else if (Instance != null && instance == null)
                    {
                        return HarmonyFieldRef(Instance);
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    if (RefGetValue == null)
                    {
                        throw new ArgumentNullException(nameof(RefGetValue));
                    }

                    if (instance != null && TType.IsInstanceOfType(instance))
                    {
                        return RefGetValue(instance);
                    }
                    else if (Instance != null && instance == null)
                    {
                        return RefGetValue(Instance);
                    }
                    else
                    {
                        return default;
                    }
                }
            }

            public void SetValue(T instance, F value)
            {
                if (UseHarmony)
                {
                    if (HarmonyFieldRef == null)
                    {
                        throw new ArgumentNullException(nameof(HarmonyFieldRef));
                    }

                    if (instance != null && TType.IsInstanceOfType(instance))
                    {
                        HarmonyFieldRef(instance) = value;
                    }
                    else if (Instance != null && instance == null)
                    {
                        HarmonyFieldRef(Instance) = value;
                    }
                }
                else
                {
                    if (RefSetValue == null)
                    {
                        throw new ArgumentNullException(nameof(RefSetValue));
                    }

                    if (instance != null && TType.IsInstanceOfType(instance))
                    {
                        RefSetValue(instance, value);
                    }
                    else if (Instance != null && instance == null)
                    {
                        RefSetValue(Instance, value);
                    }
                }
            }
        }

        public static Type GetEftType(Func<Type, bool> func)
        {
            return PatchConstants.EftTypes.Single(func);
        }

        public static MethodInfo GetEftMethod(Type type, BindingFlags flags, Func<MethodInfo, bool> func)
        {
            return type.GetMethods(flags).Single(func);
        }

        public static MethodInfo GetEftMethod(Func<Type, bool> func, BindingFlags flags, Func<MethodInfo, bool> func2)
        {
            return GetEftMethod(GetEftType(func), flags, func2);
        }
    }
}
#endif
