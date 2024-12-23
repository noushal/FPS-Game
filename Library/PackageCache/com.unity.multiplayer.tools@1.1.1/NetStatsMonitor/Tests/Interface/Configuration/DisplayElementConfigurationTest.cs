using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Interface.Configuration
{
    class DisplayElementConfigurationTest
    {
        GameObject m_RnsmGameObject;
        RuntimeNetStatsMonitor m_NetStatsMonitor;
        
        [OneTimeSetUp]
        public void Setup()
        {
            m_RnsmGameObject = new GameObject();
            m_NetStatsMonitor = m_RnsmGameObject.AddComponent<RuntimeNetStatsMonitor>();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(m_RnsmGameObject);
        }
        
        [TestCase("")]
        [TestCase("2022-05-15")]
        [TestCase("15/05/2022")]
        [TestCase("SpecialCharacter_!@#$%^&*()_-+=?")]
        [TestCase("_field_name_is_looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong_1")]
        [TestCase("0ad3823f-d501-4d47-835f-c1e54e773b59")]
        public void SetDisplayElementConfigurationLabel_WithValidInputs_LabelSetCorrectly(string input)
        {
            DisplayElementConfiguration displayElementConfiguration = new();
            displayElementConfiguration.Label = input;
            Assert.AreEqual(input,displayElementConfiguration.Label);
        }
        
        [TestCase(new[]
        {
            DisplayElementType.Counter,
            DisplayElementType.LineGraph,
            DisplayElementType.StackedAreaGraph
        })]
        public void AddRemoveDisplayElementTypeToList_GivenSupportedElementType_AddedAndRemovedSuccessfully(DisplayElementType[] displayElementTypes)
        {
            foreach (var element in displayElementTypes)
            {
                DisplayElementConfiguration displayElementConfiguration = new();
                displayElementConfiguration.Type = element;
                Assert.AreEqual(element, displayElementConfiguration.Type);

                var count = m_NetStatsMonitor.Configuration!.DisplayElements.Count;
                m_NetStatsMonitor.Configuration!.DisplayElements.Add(displayElementConfiguration);
                Assert.AreEqual(element, m_NetStatsMonitor.Configuration.DisplayElements[count].Type);
                Assert.AreEqual(count+1, m_NetStatsMonitor.Configuration.DisplayElements.Count);
                
                m_NetStatsMonitor.Configuration!.DisplayElements.Remove(displayElementConfiguration);
                Assert.AreEqual(count, m_NetStatsMonitor.Configuration!.DisplayElements.Count);
            }
        }
    }
}