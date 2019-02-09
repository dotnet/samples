[MethodImpl(MethodImplOptions.NoInlining)]
int ExecuteAndUnload(string assemblyPath, out WeakReference alcWeakRef)
{
    var alc = new TestAssemblyLoadContext();
    alcWeakRef = new WeakReference(alc, trackResurrection: true);

    Assembly a = alc.LoadFromAssemblyPath(assemblyPath);

    var args = new object[1] { new string[] {"Hello"}};
    int result = (int)a.EntryPoint.Invoke(null, args);
    alc.Unload();

    return result;
}
