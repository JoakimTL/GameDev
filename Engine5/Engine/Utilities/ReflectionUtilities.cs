using System.Reflection;

namespace Engine.Utilities;

public static class ReflectionUtilities
{

    internal static void LoadAllAssemblies()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            LoadReferencedAssembly(assembly);
        Log.Line($"Loaded {AppDomain.CurrentDomain.GetAssemblies().Count()} assemblies!", Log.Level.NORMAL);
    }

    private static void LoadReferencedAssembly(Assembly assembly)
    {
        foreach (AssemblyName name in assembly.GetReferencedAssemblies())
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName == name.FullName))
                try
                {
                    LoadReferencedAssembly(Assembly.Load(name));
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }
    }
}
