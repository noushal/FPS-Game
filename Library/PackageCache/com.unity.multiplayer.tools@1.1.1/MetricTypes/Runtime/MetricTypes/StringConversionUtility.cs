using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
    static class StringConversionUtility
    {
        /// <summary>
        /// Takes a managed string and turns it into a fixed string
        ///   If the string is null, it will return a fixed string with length 0
        ///   If the string is too large, it will be truncated
        /// </summary>
        public static FixedString64Bytes ConvertToFixedString(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            
            if (FixedString64Bytes.UTF8MaxLengthInBytes < value.Length)
            {
                var fixedString = new FixedString64Bytes();
                unsafe
                {
                    fixed (char* chars = value)
                    {
                        UTF8ArrayUnsafeUtility.Copy(fixedString.GetUnsafePtr(), out var copiedLength, FixedString64Bytes.UTF8MaxLengthInBytes, chars, value.Length);
                        fixedString.Length = copiedLength;
                    }
                }
                return fixedString;
            }

            return value;
        }
    }
}
