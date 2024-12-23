using System;
using NUnit.Framework;

namespace Unity.Multiplayer.Tools.Common.Tests
{
    [Obsolete("This test class has been marked obsolete because it was not intended to be a public API", false)]
    public class ExponentialMovingAverageTests
    {
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void InitializedCorrectlyWithOneArgument(float parameter = 0f) { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void InitializedCorrectlyWithTwoArguments(float parameter = 0f, float initialValue = 0f) { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void ValueRemainsConstantWithConstantInput() { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void ValueDecaysFromInitialValueWithZeroInput() { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void ValueRisesToMeetConstantInputWhenStartingFromZero() { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void ValueIsComputedCorrectlyInResponseToVaryingInput() { }
        
        [Obsolete("This test method has been marked obsolete because it was not intended to be part of the public API", false)]
        public void ApproximatingSimpleMovingAverageIsCorrect(int sampleCount) {}
    }
}