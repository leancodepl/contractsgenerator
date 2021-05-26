using Xunit;
using static LeanCode.ContractsGenerator.Tests.TypeRefExtensions;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased
{
    public class Properties
    {
        [Fact]
        public void Properties_with_known_types()
        {
            @"public class Dto
            {
                public int A { get; set; }
                public ulong B { get; }
                public string C { get; set; }
                public bool D { get; private set; }
                public Uri E { get; set; }
                public Date F { get; set; }
                public Time G { get; set; }
                public DateTime H { get; set; }
                public DateTimeOffset I { get; set; }
                public Guid J { get; set; }
                public float K { get; set; }
                public double L { get; set; }
            }"
                .Compiles()
                .WithDto("Dto")
                    .WithProperty("A", Known(KnownType.Int32))
                    .WithProperty("B", Known(KnownType.Uint64))
                    .WithProperty("C", Known(KnownType.String))
                    .WithProperty("D", Known(KnownType.Boolean))
                    .WithProperty("E", Known(KnownType.Uri))
                    .WithProperty("F", Known(KnownType.Date))
                    .WithProperty("G", Known(KnownType.Time))
                    .WithProperty("H", Known(KnownType.DateTime))
                    .WithProperty("I", Known(KnownType.DateTimeOffset))
                    .WithProperty("J", Known(KnownType.Guid))
                    .WithProperty("K", Known(KnownType.Float))
                    .WithProperty("L", Known(KnownType.Double));
        }

        [Fact]
        public void Properties_with_composite_types()
        {
            @"public class Dto
            {
                public IReadOnlyList<int> A { get; set; }
                public IList<int> B { get; set; }
                public List<int> C { get; set; }
                public int[] D { get; set; }
                public IReadOnlyDictionary<int, string> E { get; set; }
                public IDictionary<int, string> F { get; set; }
                public Dictionary<int, string> G { get; set; }
            }"
                .Compiles()
                .WithDto("Dto")
                    .WithProperty("A", Array(Known(KnownType.Int32)))
                    .WithProperty("B", Array(Known(KnownType.Int32)))
                    .WithProperty("C", Array(Known(KnownType.Int32)))
                    .WithProperty("D", Array(Known(KnownType.Int32)))
                    .WithProperty("E", Map(Known(KnownType.Int32), Known(KnownType.String)))
                    .WithProperty("F", Map(Known(KnownType.Int32), Known(KnownType.String)))
                    .WithProperty("G", Map(Known(KnownType.Int32), Known(KnownType.String)));
        }
    }
}
