using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace AssemblyLoading
{
    static class Usage
    {
        public const string DefaultContext = "default";
        public const string CustomContext = "custom";

        public const string AssemblyLoadContextLoad  = "alc-load";
        public const string AssemblyLoadContextResolvingDefault = "alc-resolving-default";
        public const string AssemblyLoadContextResolving = "alc-resolving";
        public const string AppDomainAssemblyResolve= "appdomain-assemblyresolve";

        public static void Print()
        {
            const int alignment = -28;
            Console.WriteLine($@"
Usage:
  AssemblyLoading <context> [<extension-point>]

context:
  {DefaultContext}  Use the default AssemblyLoadContext
  {CustomContext}   Use a new custom AssemblyLoadContext

extension-point:
  {AssemblyLoadContextLoad,alignment} AssemblyLoadContext.Load loads the assembly - if <context> is 'default', this has no effect
  {AssemblyLoadContextResolvingDefault,alignment} AssemblyLoadContext.Resolving event handler for the default AssemblyLoadContext loads the assembly
  {AssemblyLoadContextResolving,alignment} AssemblyLoadContext.Resolving event handler loads the assembly - if <context> is 'default', this is equivalent to {AssemblyLoadContextResolvingDefault}
  {AppDomainAssemblyResolve,alignment} AppDomain.AssemblyResolve event handler loads the assembly

The <extension-point> specifies which extension point will successfully load the library.
If not specified, the library will fail to load.");
        }

        public static bool IsValidExtensionPoint(string value) => value switch
        {
            AssemblyLoadContextLoad
                or AssemblyLoadContextResolving
                or AssemblyLoadContextResolvingDefault
                or AppDomainAssemblyResolve => true,
            _ => false
        };
    }

    class Program
    {
        private const string libraryName = "MyLibrary";
        private static string libraryPath = Path.Combine(AppContext.BaseDirectory, $"{libraryName}.dll");

        private static AssemblyLoadContext alc;
        private static string successfulExtensionPoint;

        public class CustomALC : AssemblyLoadContext
        {
            public CustomALC() : base(nameof(CustomALC))
            { }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                Console.WriteLine($@"
AssemblyLoadContext.Load
  - Name: {assemblyName}
  - Load context: {this}");

                if (Program.successfulExtensionPoint != Usage.AssemblyLoadContextLoad || libraryName != assemblyName.Name)
                    return null;

                return LoadFromAssemblyPath(Program.libraryPath);
            }
        }

        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0] == "/d")
            {
                Console.WriteLine("Waiting for any key...");
                Console.ReadLine();
                args = args[1..^0];
            }

            if (args.Length != 1 && args.Length != 2)
            {
                Usage.Print();
                return;
            }

            alc = args[0] switch
            {
                Usage.DefaultContext => AssemblyLoadContext.Default,
                Usage.CustomContext => new CustomALC(),
                _ => null
            };
            if (alc == null)
            {
                Usage.Print();
                return;
            }

            if (args.Length == 2)
            {
                if (!Usage.IsValidExtensionPoint(args[1]))
                {
                    Usage.Print();
                    return;
                }

                successfulExtensionPoint = args[1];
            }

            var assemblyToLoad = new AssemblyName(libraryName);

            alc.Resolving += OnAssemblyLoadContextResolving;
            AppDomain.CurrentDomain.AssemblyResolve += OnAppDomainAssemblyResolve;
            if (alc != AssemblyLoadContext.Default)
                AssemblyLoadContext.Default.Resolving += OnAssemblyLoadContextResolving;

            try
            {
                // Load assembly that is known to the application - part of Microsoft.NETCore.App framework
                LoadAssembly(new AssemblyName("System.Xml"));

                // Load assembly that is not known to the application - not directly referenced or part of a framework reference
                LoadAssembly(assemblyToLoad);
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Console.WriteLine(e);
            }

            alc.Resolving -= OnAssemblyLoadContextResolving;
            AppDomain.CurrentDomain.AssemblyResolve -= OnAppDomainAssemblyResolve;
            if (alc != AssemblyLoadContext.Default)
                AssemblyLoadContext.Default.Resolving -= OnAssemblyLoadContextResolving;
        }

        private static void LoadAssembly(AssemblyName assemblyToLoad)
        {
            Console.WriteLine($@"
=== Loading '{assemblyToLoad}' ===");

            var asm = alc.LoadFromAssemblyName(assemblyToLoad);

            Console.WriteLine($@"
Successfully loaded assembly:
  - Assembly: {asm}
  - Load context: {AssemblyLoadContext.GetLoadContext(asm)}");
        }

        private static Assembly OnAssemblyLoadContextResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            Console.WriteLine($@"
AssemblyLoadContext.Resolving event handler
  - Name: {assemblyName};
  - Load context: {context}");

            if (!((successfulExtensionPoint == Usage.AssemblyLoadContextResolving && context == alc)
                    || (successfulExtensionPoint == Usage.AssemblyLoadContextResolvingDefault && context == AssemblyLoadContext.Default))
                || !libraryName.Equals(assemblyName.Name))
            {
                return null;
            }

            return context.LoadFromAssemblyPath(libraryPath);
        }

        private static Assembly OnAppDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            Console.WriteLine($@"
AppDomain.AssemblyResolve event handler
  - Name: {assemblyName}");

            if (successfulExtensionPoint != Usage.AppDomainAssemblyResolve || !libraryName.Equals(assemblyName.Name))
                return null;

            return alc.LoadFromAssemblyPath(libraryPath);
        }
    }
}
