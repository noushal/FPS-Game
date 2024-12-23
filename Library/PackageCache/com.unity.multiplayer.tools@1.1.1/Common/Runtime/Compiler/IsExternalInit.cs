
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Workaround to enable use of C# record syntax as described here:
    /// https://docs.unity3d.com/2021.2/Documentation/Manual/CSharpCompiler.html
    /// </summary>
    /// <remarks>
    /// This will not affect user code, which does not have access to this assembly
    /// via ExposeInternalsTo
    /// </remarks>
    internal static class IsExternalInit
    {

    }
}