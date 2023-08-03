using Xunit;
using CompilationTagGenerator = LeanCode.ContractsGenerator.Generation.NotificationTagGenerator;
using ContractsTagGenerator = LeanCode.Contracts.NotificationTagGenerator;

namespace LeanCode.ContractsGenerator.Tests.Notifications;

public class NotificationTagGenerators
{
    private readonly string class1SimpleName = GetSimpleName(typeof(Class1));
    private readonly string class2SimpleName = GetSimpleName(typeof(Class2<>));
    private readonly string class3SimplName = GetSimpleName(typeof(Class3<,>));

    [Fact]
    public void Internal()
    {
        var type = typeof(Class1);
        var typeRef = TypeRefExtensions.Internal(class1SimpleName);
        var expectedTag = class1SimpleName;

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Single_generic_argument()
    {
        var type = typeof(Class2<int>);
        var typeRef = TypeRefExtensions.Internal(class2SimpleName)
            .WithArguments(TypeRefExtensions.Known(KnownType.Int32));
        var expectedTag = $"{class2SimpleName}[!Int32]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Multiple_generic_arguments()
    {
        var type = typeof(Class3<int, DateTimeOffset>);
        var typeRef = TypeRefExtensions.Internal(class3SimplName)
            .WithArguments(
                TypeRefExtensions.Known(KnownType.Int32),
                TypeRefExtensions.Known(KnownType.DateTimeOffset));
        var expectedTag = $"{class3SimplName}[!Int32,!DateTimeOffset]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Nested_generic_arguments()
    {
        var type = typeof(Class3<Class2<object>, Class2<int[]>>);
        var typeRef = TypeRefExtensions.Internal(class3SimplName)
            .WithArguments(
                TypeRefExtensions.Internal(class2SimpleName)
                    .WithArguments(TypeRefExtensions.Known(KnownType.Object)),
                TypeRefExtensions.Internal(class2SimpleName)
                    .WithArguments(TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32))));
        var expectedTag = $"{class3SimplName}[{class2SimpleName}[!Object],{class2SimpleName}[!Array[!Int32]]]";

        VerifyGenerators(type, typeRef, expectedTag);
    }

    [Fact]
    public void Map()
    {
        var type = typeof(Dictionary<Guid[], Class2<TimeSpan>>);
        var typeRef = TypeRefExtensions.Map(
            TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Guid)),
            TypeRefExtensions.Internal(class2SimpleName)
                .WithArguments(TypeRefExtensions.Known(KnownType.TimeSpan)));
        var expectedTag = $"!Map[!Array[!Guid],{class2SimpleName}[!TimeSpan]]";

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

    private static string GetSimpleName(Type type)
    {
        var typeName = type.FullName!;
        var backtickIndex = typeName.IndexOf('`');

        if (backtickIndex > 0)
        {
            typeName = typeName[..backtickIndex];
        }

        return typeName;
    }
}

public class Class1 { }

public class Class2<T> { }

public class Class3<T1, T2> { }
