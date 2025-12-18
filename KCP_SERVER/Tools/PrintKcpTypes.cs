using System;
using System.IO;
using System.Linq;
using System.Reflection;

public static class PrintKcpTypes
{
    public static void Dump()
    {
        var dll = @"C:\Users\zyrea\.nuget\packages\kcp-csharp\1.0.8\lib\net8.0\Kcp-CSharp.dll";

        var asm = Assembly.LoadFrom(dll);

        var types = asm.GetExportedTypes()
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name)
            .Select(t => $"{t.Namespace} :: {t.Name}");

        Console.WriteLine("=== KCP DLL EXPORTED TYPES ===");
        Console.WriteLine(string.Join(Environment.NewLine, types));
    }
}
