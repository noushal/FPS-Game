using System;

namespace Unity.Multiplayer.Tools.NetStats
{
    /// <summary>
    /// For internal use.
    /// This attribute is automatically added to assemblies that use types from
    /// the multiplayer tools package that require code generation to work correctly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyRequiresTypeRegistrationAttribute : Attribute
    {
    }
}