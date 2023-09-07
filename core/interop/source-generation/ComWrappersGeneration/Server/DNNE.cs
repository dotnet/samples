namespace DNNE;

/// <summary>
/// Provide C code to be defined early in the generated C header file.
/// </summary>
/// <remarks>
/// This attribute is respected on an exported method declaration or on a parameter for the method.
/// The following header files will be included prior to the code being defined.
///   - stddef.h
///   - stdint.h
///   - dnne.h
/// </remarks>
internal class C99DeclCodeAttribute : System.Attribute
{
    public C99DeclCodeAttribute(string code) { }
}

/// <summary>
/// Define the C type to be used.
/// </summary>
/// <remarks>
/// The level of indirection should be included in the supplied string.
/// </remarks>
internal class C99TypeAttribute : System.Attribute
{
    public C99TypeAttribute(string code) { }
}
