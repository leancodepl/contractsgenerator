using LeanCode.Contracts;

namespace Exclusions
{
    [ExcludeFromContractsGeneration]
    public interface IExcludedInterface { }

    [ExcludeFromContractsGeneration]
    public class ExcludedClass { }

    [ExcludeFromContractsGeneration]
    public struct ExcludedStruct { }

    [ExcludeFromContractsGeneration]
    public enum ExcludedEnum
    {
        None = 0,
    }

    public class IncludedDTO
    {
        [ExcludeFromContractsGeneration]
        public int ExcludedProperty { get; set; }
        public int IncludedProperty { get; set; }
    }
}
