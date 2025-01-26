using System.Reflection;
using TestRunner;
using Tests;

public class Progarm
{
    public static void Main(string[] args)
    {
        var testRunner = new TestsRunner();
        var assembly = Assembly.Load(nameof(Tests)); // you can load here any assembly you want 
        testRunner.RunTests(assembly);

    }
}