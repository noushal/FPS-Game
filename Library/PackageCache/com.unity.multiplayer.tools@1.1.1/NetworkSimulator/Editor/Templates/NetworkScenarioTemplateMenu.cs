using UnityEditor;

namespace Unity.Multiplayer.Tools.NetworkSimulator.Editor.Templates
{
    static class NetworkScenarioTemplateMenu
    {
        const string k_TemplateFolderPath = "Packages/com.unity.multiplayer.tools/NetworkSimulator/Editor/Templates/";
        const string k_NewScenarioTaskTemplate = "NewScenarioTask.cs.txt";
        const string k_NewScenarioTaskFilename = "NewScenarioTask.cs";
        const string k_NewScenarioBehaviourTemplate = "NewScenarioBehaviour.cs.txt";
        const string k_NewScenarioBehaviourFilename = "NewScenarioBehaviour.cs";

        [MenuItem("Assets/Create/Multiplayer/Network Scenario Task")]
        public static void CreateNewScenarioTask()
        {
            const string templatePath = k_TemplateFolderPath + k_NewScenarioTaskTemplate;
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, k_NewScenarioTaskFilename);
        }

        [MenuItem("Assets/Create/Multiplayer/Network Scenario Behaviour")]
        public static void CreateNewScenarioUpdateHandler()
        {
            const string templatePath = k_TemplateFolderPath + k_NewScenarioBehaviourTemplate;
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, k_NewScenarioBehaviourFilename);
        }
    }
}
