using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Tests;
using Shouldly;
namespace TestRunner
{
    public class TestsRunner
    {
        #region Methods
        public void RunTests(Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(t => t.IsDefined(typeof(SubjectAttribute)));
            foreach (var type in types)
            {
                var subject = type.GetCustomAttribute<SubjectAttribute>()._subject;

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Running tests for {0}", subject);

                RunTestForType(type);
            }
        }
        void RunTestForType(Type type)
        {
            var allFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).ToList();
            var shouldComponets = allFields.Where(f => f.FieldType == typeof(ItShould)).ToArray();
            var becauseComponents = allFields.Where(f => f.FieldType == typeof(Because)).ToArray();
            var contextComponets = allFields.Where(f => f.FieldType == typeof(Context)).ToArray();

            var obj = Activator.CreateInstance(type);


            RunContextComponents(contextComponets, obj);
            RunBecauseComponents(becauseComponents, obj);
            RunShouldComponents(shouldComponets, obj);

        }
        void RunContextComponents(FieldInfo[] components, object obj)
        {
            foreach (var field in components)
            {
                var delg = (Context)(field.GetValue(obj));
                delg.Invoke();
            }
        }
        void RunShouldComponents(FieldInfo[] components, object obj)
        {
            foreach (var field in components) {
                var delg = (ItShould)field.GetValue(obj);

                try
                {
                    delg.Invoke();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\t- {field.Name} test successed");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\t-Falied for {field.Name}");
                    Console.WriteLine($"\t{ex.Message.ToString()}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }
        void RunBecauseComponents(FieldInfo[] components, object obj)
        {
            foreach (var field in components)
            {
                var delg = (Because)field.GetValue(obj);
                    delg.Invoke();
            }
        }
        #endregion
    }
}
