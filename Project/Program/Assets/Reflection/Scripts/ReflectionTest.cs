using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Reflection.Scripts
{
    public class ReflectionTest : MonoBehaviour
    {
        private List<TestClass> testClasses;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test1());
        }

        IEnumerator Test1()
        {
            yield return new WaitForSeconds(10);
            
            testClasses = new List<TestClass>();
            TestSet();
            yield return new WaitForSeconds(1);
            TestGet();
        }

        private void TestSet()
        {
            for (int i = 0; i < 100000; ++i)
            {
                testClasses.Add(new TestClass());
            }

            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var aa = classType.GetField("AA", BindingFlags.Instance | BindingFlags.Public);

            var t1 = Time.realtimeSinceStartup;
            for (int i = 0; i < 100000; ++i)
            {
                aa.SetValue(testClasses[i], 1);
            }

            var t2 = Time.realtimeSinceStartup;

            var t3 = Time.realtimeSinceStartup;
            for (int i = 0; i < 100000; ++i)
            {
                testClasses[i].AA = 2;
            }

            var t4 = Time.realtimeSinceStartup;


            Debug.Log($"Set t2-t1={t2 - t1}   t4-t3 = {t4 - t3}");
        }
        
        private void TestGet()
        {
            for (int i = 0; i < 100000; ++i)
            {
                testClasses.Add(new TestClass());
            }

            Assembly assembly = Assembly.GetAssembly((typeof(TestClass)));

            var classType = assembly.GetType("Reflection.Scripts.TestClass");
            var aa = classType.GetField("AA", BindingFlags.Instance | BindingFlags.Public);

            var t1 = Time.realtimeSinceStartup;
            int temp;
            for (int i = 0; i < 100000; ++i)
            {
                temp = (int) aa.GetValue(testClasses[i]);
            }

            var t2 = Time.realtimeSinceStartup;

            var t3 = Time.realtimeSinceStartup;
            for (int i = 0; i < 100000; ++i)
            {
                temp = testClasses[i].AA;
            }

            var t4 = Time.realtimeSinceStartup;


            Debug.Log($"Get t2-t1={t2 - t1}   t4-t3 = {t4 - t3}");
        }
    }
}
