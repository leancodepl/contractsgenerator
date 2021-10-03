using Google.Protobuf.Collections;

namespace LeanCode.ContractsGenerator.Generation
{
    internal static class IEnumerableExtensions
    {
        public static void SaveToRepeatedField<T>(this IEnumerable<T> src, RepeatedField<T> output)
        {
            output.AddRange(src);
        }
    }
}
