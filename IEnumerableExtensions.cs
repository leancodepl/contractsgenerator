using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace LeanCode.ContractsGeneratorV2
{
    internal static class IEnumerableExtensions
    {
        public static void SaveToRepeatedField<T>(this IEnumerable<T> src, RepeatedField<T> output)
        {
            output.AddRange(src);
        }
    }
}
