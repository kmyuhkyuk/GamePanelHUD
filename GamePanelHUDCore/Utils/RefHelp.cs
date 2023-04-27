#if !UNITY_EDITOR
using Aki.Reflection.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GamePanelHUDCore.Utils
{
    public static class RefHelp
    {
        public static Func<T, F> ObjectFieldGetAccess<T, F>(FieldInfo fieldinfo)
        {
            if (fieldinfo == null)
            {
                throw new ArgumentNullException(nameof(fieldinfo));
            }

            var delegateInstanceType = typeof(T);
            var declaringType = fieldinfo.DeclaringType;

            bool isObject = typeof(F) == typeof(object);
            bool needBox;

            if (isObject && fieldinfo.FieldType.IsValueType)
            {
                needBox = true;
            }
            else
            {
                needBox = false;
            }

            var dmd = new DynamicMethod($"__get_{delegateInstanceType.Name}_fi_{fieldinfo.Name}", typeof(F), new Type[] { delegateInstanceType });

            var ilGen = dmd.GetILGenerator();
            if (fieldinfo.IsStatic)
            {
                ilGen.Emit(OpCodes.Ldsfld, fieldinfo);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                if (typeof(T) == typeof(object))
                {
                    ilGen.Emit(OpCodes.Castclass, declaringType);
                }

                ilGen.Emit(OpCodes.Ldfld, fieldinfo);
            }

            if (needBox)
            {
                ilGen.Emit(OpCodes.Box, fieldinfo.FieldType);
            }
            else if (isObject)
            {
                ilGen.Emit(OpCodes.Castclass, typeof(F));
            }

            ilGen.Emit(OpCodes.Ret);

            return (Func<T, F>)dmd.CreateDelegate(typeof(Func<T, F>));
        }

        public static Action<T, F> ObjectFieldSetAccess<T, F>(FieldInfo fieldinfo)
        {
            if (fieldinfo == null)
            {
                throw new ArgumentNullException(nameof(fieldinfo));
            }

            var delegateInstanceType = typeof(T);
            var declaringType = fieldinfo.DeclaringType;

            bool isObject = typeof(F) == typeof(object);
            bool needBox;

            if (isObject && fieldinfo.FieldType.IsValueType)
            {
                needBox = true;
            }
            else
            {
                needBox = false;
            }

            var dmd = new DynamicMethod($"__set_{delegateInstanceType.Name}_fi_{fieldinfo.Name}", null, new Type[] { delegateInstanceType, typeof(F) });

            var ilGen = dmd.GetILGenerator();
            if (fieldinfo.IsStatic)
            {
                ilGen.Emit(OpCodes.Ldarg_1);

                if (needBox)
                {
                    ilGen.Emit(OpCodes.Unbox_Any, fieldinfo.FieldType);
                }
                else if (isObject)
                {
                    ilGen.Emit(OpCodes.Castclass, typeof(F));
                }

                ilGen.Emit(OpCodes.Stsfld, fieldinfo);
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
                    ilGen.Emit(OpCodes.Unbox_Any, fieldinfo.FieldType);
                }
                else if (isObject)
                {
                    ilGen.Emit(OpCodes.Castclass, typeof(F));
                }

                ilGen.Emit(OpCodes.Stfld, fieldinfo);
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

            var delegateMethod = delegateType.GetMethod("Invoke");
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

                if (declaringType != null && declaringType.IsValueType)
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
                //DelegateparameterTypes i == parameterTypes i
                else if (delegateParameterTypes[i] == typeof(object) && isValueType)
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

            public Type InType
            {
                get
                {
                    return TType;
                }
            }

            public Type PropertyType
            {
                get
                {
                    return PropertyInfo.PropertyType;
                }
            }

            public PropertyRef(PropertyInfo propertyinfo, object instance)
            {
                if (propertyinfo == null)
                {
                    throw new Exception("PropertyInfo is null");
                }

                Init(propertyinfo, instance);
            }

            public PropertyRef(Type type, string propertyname, bool declaredonly, object instance)
            {
                BindingFlags flags = declaredonly ? AccessTools.allDeclared : AccessTools.all;

                PropertyInfo propertyInfo = type.GetProperty(propertyname, flags);
                
                if (propertyInfo == null)
                {
                    throw new Exception(propertyname + " is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string[] propertynames, bool declaredonly, object instance)
            {
                BindingFlags flags = declaredonly ? AccessTools.allDeclared : AccessTools.all;

                PropertyInfo propertyInfo = propertyInfo = propertynames.Select(x => type.GetProperty(x, flags)).FirstOrDefault(x => x != null);

                if (propertyInfo == null)
                {
                    throw new Exception(propertynames.First() + " is null");
                }

                Init(propertyInfo, instance);
            }

            private void Init(PropertyInfo propertyinfo, object instance)
            {
                PropertyInfo = propertyinfo;

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

            public static PropertyRef<T, F> Create(PropertyInfo propertyinfo, object instance)
            {
                return new PropertyRef<T, F>(propertyinfo, instance);
            }

            public static PropertyRef<T, F> Create(string propertyname, bool declaredonly = false, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertyname, declaredonly, instance);
            }

            public static PropertyRef<T, F> Create(string[] propertynames, bool declaredonly = false, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertynames, declaredonly, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string propertyname, bool declaredonly = false, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertyname, declaredonly, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string[] propertynames, bool declaredonly = false, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertynames, declaredonly, instance);
            }

            public F GetValue(T instance)
            {
                if (RefGetValue == null)
                {
                    throw new ArgumentNullException(nameof(RefGetValue));
                }

                if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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

                if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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

            public Type InType
            {
                get
                {
                    return TType;
                }
            }

            public Type FieldType
            {
                get
                {
                    return FieldInfo.FieldType;
                }
            }

            public FieldRef(FieldInfo fieldinfo, object instance)
            {
                if (fieldinfo == null)
                {
                    throw new Exception("FieldInfo is null");
                }

                Init(fieldinfo, instance);
            }

            public FieldRef(Type type, string fieldname, bool declaredonly, object instance)
            {
                BindingFlags flags = declaredonly ? AccessTools.allDeclared : AccessTools.all;

                FieldInfo fieldInfo = type.GetField(fieldname, flags);

                if (fieldInfo == null)
                {
                    throw new Exception(fieldname + " is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string[] fieldnames, bool declaredonly, object instance)
            {
                BindingFlags flags = declaredonly ? AccessTools.allDeclared : AccessTools.all;

                FieldInfo fieldInfo = fieldnames.Select(x => type.GetField(x, flags)).FirstOrDefault(x => x != null);

                if (fieldInfo == null)
                {
                    throw new Exception(fieldnames.First() + " is null");
                }

                Init(fieldInfo, instance);
            }

            public static FieldRef<T, F> Create(FieldInfo fieldinfo, object instance = null)
            {
                return new FieldRef<T, F>(fieldinfo, instance);
            }

            public static FieldRef<T, F> Create(string fieldname, bool declaredonly = false, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldname, declaredonly, instance);
            }

            public static FieldRef<T, F> Create(string[] fieldnames, bool declaredonly = false, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldnames, declaredonly, instance);
            }

            public static FieldRef<T, F> Create(Type type, string fieldname, bool declaredonly = false, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldname, declaredonly, instance);
            }

            public static FieldRef<T, F> Create(Type type, string[] fieldnames, bool declaredonly = false, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldnames, declaredonly, instance);
            }

            private void Init(FieldInfo fieldinfo, object instance = null)
            {
                FieldInfo = fieldinfo;

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

                    if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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

                    if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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

                    if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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

                    if (instance != null && TType.IsAssignableFrom(instance.GetType()))
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
