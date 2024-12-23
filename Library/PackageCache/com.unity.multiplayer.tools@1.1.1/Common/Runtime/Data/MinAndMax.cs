namespace Unity.Multiplayer.Tools.Common
{
    internal struct MinAndMax
    {
        public MinAndMax(float min, float max)
        {
            Min = min;
            Max = max;
        }
        public float Min { get; set; }
        public float Max { get; set; }
    }
}