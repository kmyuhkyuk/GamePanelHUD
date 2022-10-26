using Aki.Reflection.Utils;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GamePanelHUDCore.Utils
{
    public class RefHelp
    {
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

            public Type InType;

            public Type PropertyType;

            public PropertyRef(PropertyInfo propertyinfo, object instance = null)
            {
                if (propertyinfo == null)
                {
                    throw new Exception("PropertyInfo is null");
                }

                Init(propertyinfo, instance);
            }

            public PropertyRef(Type type, string propertyname, object instance = null)
            {
                PropertyInfo propertyInfo = type.GetProperty(propertyname, AccessTools.all);

                if (propertyInfo == null)
                {
                    throw new Exception(propertyname + " is null");
                }

                Init(propertyInfo, instance);
            }

            public PropertyRef(Type type, string[] propertynames, object instance = null)
            {
                PropertyInfo propertyInfo = propertynames.Select(x => type.GetProperty(x, AccessTools.all)).FirstOrDefault(x => x != null);

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

                InType = TType;

                PropertyType = PropertyInfo.PropertyType;

                Instance = (T)instance;

                if (PropertyInfo.CanRead)
                {
                    GetMethodInfo = PropertyInfo.GetGetMethod(true);

                    RefGetValue = ObjectMethodDelegate<Func<T, F>>(GetMethodInfo);
                }

                if (PropertyInfo.CanWrite)
                {
                    SetMethodInfo = PropertyInfo.GetSetMethod(true);

                    RefSetValue = ObjectMethodDelegate<Action<T, F>>(SetMethodInfo);
                }
            }

            public static PropertyRef<T, F> Create(PropertyInfo propertyinfo, object instance = null)
            {
                return new PropertyRef<T, F>(propertyinfo, instance);
            }

            public static PropertyRef<T, F> Create(string propertyname, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertyname, instance);
            }

            public static PropertyRef<T, F> Create(string[] propertynames, object instance = null)
            {
                return new PropertyRef<T, F>(typeof(T), propertynames, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string propertyname, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertyname, instance);
            }

            public static PropertyRef<T, F> Create(Type type, string[] propertynames, object instance = null)
            {
                return new PropertyRef<T, F>(type, propertynames, instance);
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

            private FieldInfo FieldInfo;

            private Type TType;

            private T Instance;

            public Type InType;

            public Type FieldType;

            public FieldRef(FieldInfo fieldinfo, object instance = null)
            {
                if (fieldinfo == null)
                {
                    throw new Exception("FieldInfo is null");
                }

                Init(fieldinfo, instance);
            }

            public FieldRef(Type type, string fieldname, object instance = null)
            {
                FieldInfo fieldInfo = type.GetField(fieldname, AccessTools.all);

                if (fieldInfo == null)
                {
                    throw new Exception(fieldInfo + " is null");
                }

                Init(fieldInfo, instance);
            }

            public FieldRef(Type type, string[] fieldnames, object instance = null)
            {
                FieldInfo fieldInfo = fieldnames.Select(x => type.GetField(x, AccessTools.all)).FirstOrDefault(x => x != null);

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

            public static FieldRef<T, F> Create(string fieldname, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldname, instance);
            }

            public static FieldRef<T, F> Create(string[] fieldnames, object instance = null)
            {
                return new FieldRef<T, F>(typeof(T), fieldnames, instance);
            }

            public static FieldRef<T, F> Create(Type type, string fieldname, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldname, instance);
            }

            public static FieldRef<T, F> Create(Type type, string[] fieldnames, object instance = null)
            {
                return new FieldRef<T, F>(type, fieldnames, instance);
            }

            private void Init(FieldInfo fieldinfo, object instance = null)
            {
                FieldInfo = fieldinfo;

                TType = FieldInfo.DeclaringType;

                InType = TType;

                FieldType = FieldInfo.FieldType;

                Instance = (T)instance;

                HarmonyFieldRef = AccessTools.FieldRefAccess<T, F>(FieldInfo);
            }

            public F GetValue(T instance)
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

            public void SetValue(T instance, F value)
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
