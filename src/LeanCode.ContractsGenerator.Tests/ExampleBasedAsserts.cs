using System.Text.RegularExpressions;
using LeanCode.ContractsGenerator.Generation;
using Xunit;
using Xunit.Sdk;

namespace LeanCode.ContractsGenerator.Tests;

public static class ExampleBasedAsserts
{
    private static AssertedStatement With(this AssertedExport export, string name)
    {
        return new(
            export.Export,
            Assert.Single(export.Export.Statements, stmt => stmt.Name == name)
        );
    }

    public static T Without<T>(this T export, string name)
        where T : AssertedExport
    {
        Assert.DoesNotContain(export.Export.Statements, stmt => stmt.Name == name);
        return export;
    }

    public static AssertedStatement WithSingle(this AssertedExport stmt)
    {
        return new(stmt.Export, Assert.Single(stmt.Export.Statements));
    }

    public static AssertedExport WithStatements(this AssertedExport stmt, int count)
    {
        Assert.Equal(count, stmt.Export.Statements.Count);
        return stmt;
    }

    public static AssertedCommand WithCommand(this AssertedExport export, string name)
    {
        return export.With(name).Command(name);
    }

    public static AssertedQuery WithQuery(this AssertedExport export, string name)
    {
        return export.With(name).Query(name);
    }

    public static AssertedOperation WithOperation(this AssertedExport export, string name)
    {
        return export.With(name).Operation(name);
    }

    public static AssertedTopic WithTopic(this AssertedExport export, string name)
    {
        return export.With(name).Topic(name);
    }

    public static AssertedDto WithDto(this AssertedExport export, string name)
    {
        return export.With(name).Dto(name);
    }

    public static AssertedEnum WithEnum(this AssertedExport export, string name)
    {
        return export.With(name).Enum(name);
    }

    public static AssertedCommand Command(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Command);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement, stmt.Statement.Command.TypeDescriptor);
    }

    public static AssertedQuery Query(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Query);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement, stmt.Statement.Query.TypeDescriptor);
    }

    public static AssertedOperation Operation(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Operation);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement, stmt.Statement.Operation.TypeDescriptor);
    }

    public static AssertedTopic Topic(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Topic);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement, stmt.Statement.Topic.TypeDescriptor);
    }

    public static AssertedDto Dto(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Dto);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement, stmt.Statement.Dto.TypeDescriptor);
    }

    public static AssertedEnum Enum(this AssertedStatement stmt, string name)
    {
        Assert.NotNull(stmt.Statement.Enum);
        Assert.Equal(name, stmt.Statement.Name);
        return new(stmt.Export, stmt.Statement);
    }

    public static AssertedStatement Commented(this AssertedStatement stmt, string comment)
    {
        Assert.Equal(comment, stmt.Statement.Comment);
        return stmt;
    }

    public static AssertedErrors WithError(this AssertedErrors errors, string code)
    {
        Assert.Contains(errors.Errors, e => e.Code == code);
        return errors;
    }

    public static AssertedErrors WithError(this AssertedErrors errors, string code, string path)
    {
        try
        {
            Assert.Contains(errors.Errors, e => e.Code == code && e.Context.Path == path);
        }
        catch (ContainsException ex)
        {
            throw new ContainsException($"Error {code} at {path}.", ex.Actual);
        }

        return errors;
    }

    public static AssertedErrors WithError(
        this AssertedErrors errors,
        string code,
        string path,
        string messagePattern
    )
    {
        var regex = new Regex(messagePattern);
        try
        {
            Assert.Contains(
                errors.Errors,
                e => e.Code == code && e.Context.Path == path && regex.IsMatch(e.Message)
            );
        }
        catch (ContainsException ex)
        {
            throw new ContainsException(
                $"Error {code} at {path} that matches '{messagePattern}'.",
                ex.Actual
            );
        }

        return errors;
    }

    public static AssertedErrors WithErrorNumber(this AssertedErrors errors, int count)
    {
        Assert.Equal(count, errors.Errors.Count);
        return errors;
    }

    public static AssertedQuery WithReturnType(this AssertedQuery stmt, TypeRef typeRef)
    {
        Assert.Equal(typeRef, stmt.Statement.Query.ReturnType);
        return stmt;
    }

    public static AssertedOperation WithReturnType(this AssertedOperation stmt, TypeRef typeRef)
    {
        Assert.Equal(typeRef, stmt.Statement.Operation.ReturnType);
        return stmt;
    }

    public static AssertedTopic WithNotification(
        this AssertedTopic stmt,
        NotificationTypeRef typeRef
    )
    {
        Assert.Contains(typeRef, stmt.Statement.Topic.Notifications);
        return stmt;
    }

    public static AssertedCommand WithErrorCode(this AssertedCommand stmt, ErrorCode errorCode)
    {
        Assert.Contains(errorCode, stmt.Statement.Command.ErrorCodes);
        return stmt;
    }

    public static AssertedEnumMember WithMember(
        this AssertedEnum stmt,
        string name,
        long value,
        string comment = ""
    )
    {
        var c = Assert.Single(stmt.Statement.Enum.Members, c => c.Name == name);
        Assert.Equal(value, c.Value);
        Assert.Equal(comment, c.Comment);
        return new(stmt.Export, stmt.Statement, c);
    }

    public static AssertedEnum WithoutMember(this AssertedEnum stmt, string name)
    {
        Assert.DoesNotContain(stmt.Statement.Enum.Members, c => c.Name == name);
        return stmt;
    }

    public static T ThatExtends<T>(this T stmt, TypeRef typeRef)
        where T : AssertedType
    {
        Assert.Contains(typeRef, stmt.Descriptor.Extends);
        return stmt;
    }

    public static T WithConstant<T>(this T stmt, string name, object? value, string comment = "")
        where T : AssertedType
    {
        var c = Assert.Single(stmt.Descriptor.Constants, c => c.Name == name);
        Assert.Equal(value.ToValueRef(), c.Value);
        Assert.Equal(comment, c.Comment);
        return stmt;
    }

    public static T WithConstants<T>(this T stmt, int count)
        where T : AssertedType
    {
        Assert.Equal(count, stmt.Descriptor.Constants.Count);
        return stmt;
    }

    public static T WithNoConstants<T>(this T stmt)
        where T : AssertedType
    {
        return stmt.WithConstants(0);
    }

    public static T WithProperty<T>(this T stmt, string name, TypeRef type, string comment = "")
        where T : AssertedType
    {
        var prop = Assert.Single(stmt.Descriptor.Properties, p => p.Name == name);
        Assert.Equal(type, prop.Type);
        Assert.Equal(comment, prop.Comment);
        return stmt;
    }

    public static T WithProperty<T>(this T stmt, string name, Action<AssertedProperty> assert)
        where T : AssertedType
    {
        var prop = Assert.Single(stmt.Descriptor.Properties, p => p.Name == name);
        assert(new(prop!));
        return stmt;
    }

    public static T WithoutProperty<T>(this T stmt, string name)
        where T : AssertedType
    {
        Assert.DoesNotContain(stmt.Descriptor.Properties, p => p.Name == name);
        return stmt;
    }

    public static AssertedProperty OfType(this AssertedProperty prop, TypeRef type)
    {
        Assert.Equal(type, prop.Property.Type);
        return prop;
    }

    public static AssertedProperty WithAttribute(
        this AssertedProperty prop,
        string name,
        params AttributeArgument[] args
    )
    {
        var attr = Assert.Single(prop.Property.Attributes, a => a.AttributeName == name);
        Assert.Equal(Positional(args), Positional(attr.Argument));
        Assert.Equal(Named(args), Named(attr.Argument));
        return prop;
    }

    public static AssertedEnumMember WithAttribute(
        this AssertedEnumMember member,
        string name,
        params AttributeArgument[] args
    )
    {
        var attr = Assert.Single(member.Value.Attributes, a => a.AttributeName == name);
        Assert.Equal(Positional(args), Positional(attr.Argument));
        Assert.Equal(Named(args), Named(attr.Argument));
        return member;
    }

    public static T WithAttribute<T>(this T stmt, string name, params AttributeArgument[] args)
        where T : AssertedStatement
    {
        var attr = Assert.Single(stmt.Statement.Attributes, a => a.AttributeName == name);
        Assert.Equal(Positional(args), Positional(attr.Argument));
        Assert.Equal(Named(args), Named(attr.Argument));
        return stmt;
    }

    private static IEnumerable<AttributeArgument.Types.Positional> Positional(
        IEnumerable<AttributeArgument> args
    )
    {
        return args.Select(a => a.Positional)
            .Where(p => p is not null)
            .OrderBy(p => p.Position)
            .Cast<AttributeArgument.Types.Positional>();
    }

    private static IEnumerable<AttributeArgument.Types.Named> Named(
        IEnumerable<AttributeArgument> args
    )
    {
        return args.Select(a => a.Named)
            .Where(p => p is not null)
            .OrderBy(p => p.Name)
            .Cast<AttributeArgument.Types.Named>();
    }
}

public record AssertedExport(Export Export);

public record AssertedStatement(Export Export, Statement Statement) : AssertedExport(Export);

public record AssertedType(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedStatement(Export, Statement);

public record AssertedCommand(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedType(Export, Statement, Descriptor);

public record AssertedQuery(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedType(Export, Statement, Descriptor);

public record AssertedOperation(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedType(Export, Statement, Descriptor);

public record AssertedTopic(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedType(Export, Statement, Descriptor);

public record AssertedDto(Export Export, Statement Statement, TypeDescriptor Descriptor)
    : AssertedType(Export, Statement, Descriptor);

[System.Diagnostics.CodeAnalysis.SuppressMessage("?", "CA1711", Justification = "We don't care.")]
public record AssertedEnum(Export Export, Statement Statement)
    : AssertedStatement(Export, Statement);

public record AssertedEnumMember(Export Export, Statement Statement, EnumValue Value)
    : AssertedEnum(Export, Statement);

public record AssertedProperty(PropertyRef Property);

public record AssertedErrors(IReadOnlyList<AnalyzeError> Errors);
