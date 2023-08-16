using Xunit;
using CompilationTagGenerator = LeanCode.ContractsGenerator.Generation.NotificationTagGenerator;
using ContractsTagGenerator = LeanCode.Contracts.NotificationTagGenerator;
using OtherNamespaceDTO1 = Notifications.Internal.DTO1;

namespace LeanCode.ContractsGenerator.Tests.Notifications;

public class NotificationTagGenerators
{
    private readonly string class1Name = $"{typeof(Class1).FullName}";
    private readonly string class2Name = $"{typeof(Class2<>).Namespace}.{nameof(Class2<object>)}";
    private readonly string class3Name =
        $"{typeof(Class3<,>).Namespace}.{nameof(Class3<object, object>)}";
    private readonly string listName = $"{typeof(List<>).Namespace}.{nameof(List<object>)}";
    private readonly string dto1Name = $"{typeof(DTO1).FullName}";
    private readonly string otherNamespaceDto1Name = $"{typeof(OtherNamespaceDTO1).FullName}";

    [Fact]
    public void Simple_class()
    {
        var type = typeof(Class1);
        var typeRef = TypeRefExtensions.Internal(class1Name);
        var expectedTag = class1Name;

        CheckGeneratedTags(type, typeRef, expectedTag);
    }

    [Fact]
    public void Single_generic_argument()
    {
        var type = typeof(Class2<int>);
        var typeRef = TypeRefExtensions
            .Internal(class2Name)
            .WithArguments(TypeRefExtensions.Known(KnownType.Int32));
        var expectedTag = $"{class2Name}[!Int32]";

        CheckGeneratedTags(type, typeRef, expectedTag);
    }

    [Fact]
    public void Multiple_generic_arguments()
    {
        var type = typeof(Class3<int, DateTimeOffset>);
        var typeRef = TypeRefExtensions
            .Internal(class3Name)
            .WithArguments(
                TypeRefExtensions.Known(KnownType.Int32),
                TypeRefExtensions.Known(KnownType.DateTimeOffset)
            );
        var expectedTag = $"{class3Name}[!Int32,!DateTimeOffset]";

        CheckGeneratedTags(type, typeRef, expectedTag);
    }

    [Fact]
    public void Nested_generic_arguments()
    {
        var type = typeof(Class3<Class2<object>, Class2<int[]>>);
        var typeRef = TypeRefExtensions
            .Internal(class3Name)
            .WithArguments(
                TypeRefExtensions
                    .Internal(class2Name)
                    .WithArguments(TypeRefExtensions.Known(KnownType.Object)),
                TypeRefExtensions
                    .Internal(class2Name)
                    .WithArguments(
                        TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32))
                    )
            );
        var expectedTag = $"{class3Name}[{class2Name}[!Object],{class2Name}[!Array[!Int32]]]";

        CheckGeneratedTags(type, typeRef, expectedTag);
    }

    [Fact]
    public void Map()
    {
        var type = typeof(Dictionary<Guid[], Class2<TimeSpan>>);
        var typeRef = TypeRefExtensions.Map(
            TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Guid)),
            TypeRefExtensions
                .Internal(class2Name)
                .WithArguments(TypeRefExtensions.Known(KnownType.TimeSpan))
        );
        var expectedTag = $"!Map[!Array[!Guid],{class2Name}[!TimeSpan]]";

        CheckGeneratedTags(type, typeRef, expectedTag);

        var interfaceType1 = typeof(IDictionary<Guid[], Class2<TimeSpan>>);
        CheckGeneratedTags(interfaceType1, typeRef, expectedTag);

        var interfaceType2 = typeof(IReadOnlyDictionary<Guid[], Class2<TimeSpan>>);
        CheckGeneratedTags(interfaceType2, typeRef, expectedTag);
    }

    [Fact]
    public void Array()
    {
        var type = typeof(Dictionary<int, string>[][]);
        var typeRef = TypeRefExtensions.Array(
            TypeRefExtensions.Array(
                TypeRefExtensions.Map(
                    TypeRefExtensions.Known(KnownType.Int32),
                    TypeRefExtensions.Known(KnownType.String)
                )
            )
        );
        var expectedTag = "!Array[!Array[!Map[!Int32,!String]]]";

        CheckGeneratedTags(type, typeRef, expectedTag);

        var listType = typeof(System.Collections.Generic.List<System.Collections.Generic.List<
            Dictionary<int, string>
        >>);
        CheckGeneratedTags(listType, typeRef, expectedTag);

        var enumerableType = typeof(IEnumerable<IEnumerable<Dictionary<int, string>>>);
        CheckGeneratedTags(enumerableType, typeRef, expectedTag);
    }

    [Fact]
    public void Known_type_name_clashes()
    {
        var type1 = typeof(List<int>);
        var type2 = typeof(System.Collections.Generic.List<int>);

        CheckIfTagsAreDifferent(type1, type2);

        var typeRef1 = TypeRefExtensions
            .Internal(listName)
            .WithArguments(TypeRefExtensions.Known(KnownType.Int32));
        var typeRef2 = TypeRefExtensions.Array(TypeRefExtensions.Known(KnownType.Int32));

        CheckIfTagsAreDifferent(typeRef1, typeRef2);

        CheckGeneratedTags(type1, typeRef1, expectedTag: $"{listName}[!Int32]");
        CheckGeneratedTags(type2, typeRef2, expectedTag: "!Array[!Int32]");
    }

    [Fact]
    public void Class_name_clashes()
    {
        var type1 = typeof(DTO1);
        var type2 = typeof(OtherNamespaceDTO1);

        CheckIfTagsAreDifferent(type1, type2);

        var typeRef1 = TypeRefExtensions.Internal(dto1Name);
        var typeRef2 = TypeRefExtensions.Internal(otherNamespaceDto1Name);

        CheckIfTagsAreDifferent(typeRef1, typeRef2);

        CheckGeneratedTags(type1, typeRef1, dto1Name);
        CheckGeneratedTags(type2, typeRef2, otherNamespaceDto1Name);
    }

    private static void CheckGeneratedTags(Type type, TypeRef typeRef, string expectedTag)
    {
        Assert.Equal(expected: expectedTag, actual: ContractsTagGenerator.Generate(type));

        Assert.Equal(expected: expectedTag, actual: CompilationTagGenerator.Generate(typeRef));
    }

    private static void CheckIfTagsAreDifferent(Type type1, Type type2)
    {
        Assert.NotEqual(
            ContractsTagGenerator.Generate(type1),
            ContractsTagGenerator.Generate(type2)
        );
    }

    private static void CheckIfTagsAreDifferent(TypeRef typeRef1, TypeRef typeRef2)
    {
        Assert.NotEqual(
            CompilationTagGenerator.Generate(typeRef1),
            CompilationTagGenerator.Generate(typeRef2)
        );
    }
}

public class Class1 { }

public class Class2<T> { }

public class Class3<T1, T2> { }

public class List<T> { }

public class DTO1 { }
