using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;

namespace Unity.Multiplayer.Tools.Editor
{
    // More Info:
    // https://docs.unity3d.com/2020.1/Documentation/ScriptReference/Build.IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile.html
    // https://github.com/jilleJr/Newtonsoft.Json-for-Unity/wiki/Reference-link.xml
    // https://forum.unity.com/threads/the-current-state-of-link-xml-in-packages.995848/
    class GenerateLinkXml : IUnityLinkerProcessor
    {
        const string k_LinkXmlPath = "Packages/com.unity.multiplayer.tools/Editor/link.xml";
        
        public int callbackOrder => 0;

        public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            return Path.GetFullPath(k_LinkXmlPath);
        }
    }
}