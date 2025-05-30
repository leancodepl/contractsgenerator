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

    public enum IncludedEnum
    {
        [ExcludeFromContractsGeneration]
        ExcludedValue = 1,
        IncludedValue = 0,
    }

    public class IncludedDTO
    {
        [ExcludeFromContractsGeneration]
        public const int ExcludedConstant = 1000;
        public const int IncludedConstant = ExcludedConstant + 1;

        [ExcludeFromContractsGeneration]
        public int ExcludedProperty { get; set; }
        public int IncludedProperty { get; set; }
    }
}
