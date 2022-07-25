using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Reflection.Scripts
{
    public delegate void SetValueDelegate(object target, object arg);
    
    public static class DynamicMethodFactory
    {
        public static SetValueDelegate CreatePropertySetter(PropertyInfo property)
        {
            if( property == null )
                throw new ArgumentNullException("property");

            if( !property.CanWrite )
                return null;

            MethodInfo setMethod = property.GetSetMethod(true);

            DynamicMethod dm = new DynamicMethod("PropertySetter", null,
                new Type[] { typeof(object), typeof(object) }, property.DeclaringType, true);

            ILGenerator il = dm.GetILGenerator();

            if( !setMethod.IsStatic ) {
                il.Emit(OpCodes.Ldarg_0);
            }
            il.Emit(OpCodes.Ldarg_1);

            EmitCastToReference(il, property.PropertyType);
            if( !setMethod.IsStatic && !property.DeclaringType.IsValueType ) {
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
            }
            else
                il.EmitCall(OpCodes.Call, setMethod, null);

            il.Emit(OpCodes.Ret);

            return (SetValueDelegate)dm.CreateDelegate(typeof(SetValueDelegate));
        }
    
        private static void EmitCastToReference(ILGenerator il, Type type)
        {
            if( type.IsValueType )
                il.Emit(OpCodes.Unbox_Any, type);
            else
                il.Emit(OpCodes.Castclass, type);
        }
    }
    
    public class PropertyWrapper<T>
    {
        private Action<T> setter;
        private Func<T> getter;

        public T Value
        {
            get
            {
                return getter();
            }
            set
            {
                setter(value);
            }
        }

        public PropertyWrapper(object target, PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetSetMethod();
            var @delegate = Delegate.CreateDelegate(typeof(Action<T>), target, methodInfo);
            setter = (Action<T>)@delegate;

            methodInfo = propertyInfo.GetGetMethod();
            @delegate = Delegate.CreateDelegate(typeof(Func<T>), target, methodInfo);
            getter = (Func<T>)@delegate;
        }
    }
    
    public class ReflectionTest : MonoBehaviour
    {
        private List<TestClass> testClasses;

        private const int ClassCount = 100000;
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test1());
        }

        IEnumerator Test1()
        {
            yield return new WaitForSeconds(10);
            
            testClasses = new List<TestClass>();
            
            for (int i = 0; i < ClassCount; ++i)
            {
                testClasses.Add(new TestClass());
            }
            
            TestSet();
            yield return new WaitForSeconds(1);
            TestGet();
            yield return new WaitForSeconds(1);
            TestReflectionSet();
            yield return new WaitForSeconds(1);
            TestReflectionGet();
            yield return new WaitForSeconds(1);
            TestDelegateSet();
            yield return new WaitForSeconds(1);
            TestDelegateGet();
            yield return new WaitForSeconds(1);
            TestEmitSet();
        }

        private void TestReflectionGet()
        {
            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var aa = classType.GetField("AA", BindingFlags.Instance | BindingFlags.Public);

            var t1 = Time.realtimeSinceStartup;
            int temp;
            for (int i = 0; i < ClassCount; ++i)
            {
                temp = (int) aa.GetValue(testClasses[i]);
            }

            var t2 = Time.realtimeSinceStartup;
            
            Debug.Log($"Reflection Get {t2 - t1}");
        }

        private void TestReflectionSet()
        {
            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var aa = classType.GetField("AA", BindingFlags.Instance | BindingFlags.Public);

            var t1 = Time.realtimeSinceStartup;
            for (int i = 0; i < ClassCount; ++i)
            {
                aa.SetValue(testClasses[i], 1);
            }

            var t2 = Time.realtimeSinceStartup;
            Debug.Log($"Reflection Set {t2 - t1}");
        }

        private void TestSet()
        {
            var t3 = Time.realtimeSinceStartup;
            for (int i = 0; i < ClassCount; ++i)
            {
                testClasses[i].AA = 2;
            }

            var t4 = Time.realtimeSinceStartup;


            Debug.Log($"Set {t4 - t3}");
        }
        
        private void TestGet()
        {
            int temp;
            var t3 = Time.realtimeSinceStartup;
            for (int i = 0; i < ClassCount; ++i)
            {
                temp = testClasses[i].AA;
            }

            var t4 = Time.realtimeSinceStartup;


            Debug.Log($"Get {t4 - t3}");
        }

        private void TestDelegateSet()
        {
            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var bb = classType.GetProperty("BB", BindingFlags.Instance | BindingFlags.Public);

            List<PropertyWrapper<int>> pwList = new List<PropertyWrapper<int>>();
            for (int i = 0; i < ClassCount; ++i)
            {
                PropertyWrapper<int> pw = new PropertyWrapper<int>(testClasses[i], bb);
                pwList.Add(pw);
            }

            var t5 = Time.realtimeSinceStartup;
            foreach (var pw in pwList)
            {
                pw.Value = 1;
            }
            var t6 = Time.realtimeSinceStartup;
            
            Debug.Log($"Delegate Set {t6 - t5}");
        }

        private void TestDelegateGet()
        {
            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var bb = classType.GetProperty("BB", BindingFlags.Instance | BindingFlags.Public);

            List<PropertyWrapper<int>> pwList = new List<PropertyWrapper<int>>();
            for (int i = 0; i < ClassCount; ++i)
            {
                PropertyWrapper<int> pw = new PropertyWrapper<int>(testClasses[i], bb);
                pwList.Add(pw);
            }

            int temp;
            var t5 = Time.realtimeSinceStartup;
            foreach (var pw in pwList)
            {
                temp = pw.Value;
            }
            var t6 = Time.realtimeSinceStartup;
            
            Debug.Log($"Delegate Set {t6 - t5}");
        }
        
        private void TestEmitSet()
        {
            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var bb = classType.GetProperty("BB", BindingFlags.Instance | BindingFlags.Public);

            
            SetValueDelegate setter1 = DynamicMethodFactory.CreatePropertySetter(bb);

            var t7 = Time.realtimeSinceStartup;
            foreach (var pw in testClasses)
            {
                setter1(pw, 3);
            }
            var t8 = Time.realtimeSinceStartup;
            
            Debug.Log($"Emit Set {t8 - t7}");
        }
    }
}
