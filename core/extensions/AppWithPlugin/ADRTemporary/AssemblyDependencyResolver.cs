using System;
using System.Reflection;

namespace System.Runtime.Loader
{
    /// <summary>
    /// Temporary until the actual public API gets propagated through CoreFX.
    /// </summary>
    public class AssemblyDependencyResolver
    {
        private object _implementation;
        private Type _implementationType;
        private MethodInfo _resolveAssemblyPathInfo;
        private MethodInfo _resolveUnmanagedDllPathInfo;

        public AssemblyDependencyResolver(string componentAssemblyPath)
        {
            _implementationType = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyDependencyResolver");
            if (_implementationType == null)
            {
                throw new Exception(
                    "Can't find System.Runtime.Loader.AssemblyDependencyResolver in System.Private.Corlib.dll.\n" +
                    "Probably old version of corelib?");
            }
            _resolveAssemblyPathInfo = _implementationType.GetMethod(nameof(ResolveAssemblyToPath));
            _resolveUnmanagedDllPathInfo = _implementationType.GetMethod(nameof(ResolveUnmanagedDllToPath));

            try
            {
                _implementation = Activator.CreateInstance(_implementationType, componentAssemblyPath);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public string ResolveAssemblyToPath(System.Reflection.AssemblyName assemblyName)
        {
            try
            {
                return (string)_resolveAssemblyPathInfo.Invoke(_implementation, new object[] { assemblyName });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public string ResolveUnmanagedDllToPath(string unmanagedDllName)
        {
            try
            {
                return (string)_resolveUnmanagedDllPathInfo.Invoke(_implementation, new object[] { unmanagedDllName });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
    }
}
