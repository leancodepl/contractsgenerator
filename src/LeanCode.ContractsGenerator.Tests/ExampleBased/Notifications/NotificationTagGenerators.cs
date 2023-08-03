using Xunit;
using CompilationTagGenerator = LeanCode.ContractsGenerator.Generation.NotificationTagGenerator;
using ContractsTagGenerator = LeanCode.Contracts.NotificationTagGenerator;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications;

public class NotificationTagGenerators
{
    private const string Namespace = "LeanCode.ContractsGenerator.Tests.ExampleBased.Notifications";
    private const string Class1FullName = $"{Namespace}.Class1";
    private const string Class2FullName = $"{Namespace}.Class2";
    private const string Class3FullName = $"{Namespace}.Class3";

    [Fact]
    public void Internal()
    {
        var type = typeof(Class1);
        var typeRef = TypeRefExtensions.Internal(Class1FullName);
        var expectedTag = Class1FullName;

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Single_generic_argument()
    {
        var type = typeof(Class2<int>);
        var typeRef = TypeRefExtensions.Internal(Class2FullName)
            .WithArguments(TypeRefExtensions.Known(KnownType.Int32));
        var expectedTag = $"{Class2FullName}[!Int32]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Multiple_generic_arguments()
    {
        var type = typeof(Class3<int, DateTimeOffset>);
        var typeRef = TypeRefExtensions.Internal(Class3FullName)
            .WithArguments(
                TypeRefExtensions.Known(KnownType.Int32),
                TypeRefExtensions.Known(KnownType.DateTimeOffset));
        var expectedTag = $"{Class3FullName}[!Int32,!DateTimeOffset]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Nested_generic_arguments()
    {
        var type = typeof(Class3<Class2<object>, Class2<int[]>>);
        var typeRef = TypeRefExtensions.Internal(Class3FullName)
            .WithArguments(
                TypeRefExtensions.Internal(Class2FullName)
                    .WithArguments(TypeRefExtensions.Known(KnownType.Object)),
                TypeRefExtensions.Internal(Class2FullName)
                    .WithArguments(TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32))));
        var expectedTag = $"{Class3FullName}[{Class2FullName}[!Object],{Class2FullName}[!Array[!Int32]]]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Map()
    {
        var type = typeof(Dictionary<Guid[], Class2<TimeSpan>>);
        var typeRef = TypeRefExtensions.Map(
            TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Guid)),
            TypeRefExtensions.Internal(Class2FullName)
                .WithArguments(TypeRefExtensions.Known(KnownType.TimeSpan)));
        var expectedTag = $"!Map[!Array[!Guid],{Class2FullName}[!TimeSpan]]";

        VerifyGenerators(type, typeRef, expectedTag);

        var interfaceType1 = typeof(IDictionary<Guid[], Class2<TimeSpan>>);
        VerifyGenerators(interfaceType1, typeRef, expectedTag);

        var interfaceType2 = typeof(IReadOnlyDictionary<Guid[], Class2<TimeSpan>>);
        VerifyGenerators(interfaceType2, typeRef, expectedTag);
    }

    [Fact]
    public void Array()
    {
        var type = typeof(Dictionary<int, string>[][]);
        var typeRef = TypeRefExtensions.Array(
            TypeRefExtensions.Array(
                TypeRefExtensions.Map(
                    TypeRefExtensions.Known(KnownType.Int32),
                    TypeRefExtensions.Known(KnownType.String))));
        var expectedTag = "!Array[!Array[!Map[!Int32,!String]]]";

        VerifyGenerators(type, typeRef, expectedTag);

        var listType = typeof(List<List<Dictionary<int, string>>>);
        VerifyGenerators(listType, typeRef, expectedTag);

        var enumerableType = typeof(IEnumerable<IEnumerable<Dictionary<int, string>>>);
        VerifyGenerators(enumerableType, typeRef, expectedTag);
    }

    private static void VerifyGenerators(Type type, TypeRef typeRef, string expectedTag)
    {
        Assert.Equal(
            expected: expectedTag,
            actual: ContractsTagGenerator.Generate(type));

        Assert.Equal(
            expected: expectedTag,
            actual: CompilationTagGenerator.Generate(typeRef));
    }
}

public class Class1 { }

public class Class2<T> { }

public class Class3<T1, T2> { }
