using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Test
{
    class Program
    {
        static int Main(string[] args)
        {
            var types = typeof(Program).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.GetCustomAttributes(typeof(TestClassAttribute), false).Any())
                {
                    continue;
                }

                var instance = Activator.CreateInstance(type);
                foreach (var method in type.GetMethods())
                {
                    if (!method.GetCustomAttributes(typeof(TestMethodAttribute), false).Any())
                    {
                        continue;
                    }

                    var id = "T" + method.GetHashCode().ToString("x4");
                    Console.WriteLine($"Test : info {id}: {method}");
                    try
                    {
                        method.Invoke(instance, new object[0]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Test : error T0002: {ex}");
                        return 1;
                    }
                }
            }
            return 0;
        }
    }
}
