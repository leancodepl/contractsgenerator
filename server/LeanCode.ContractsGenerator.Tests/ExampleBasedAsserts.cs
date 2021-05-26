using Xunit;

namespace LeanCode.ContractsGenerator.Tests
{
    public static class ExampleBasedAsserts
    {
        private static AssertedStatement With(this Export export, string name)
        {
            return new(Assert.Single(export.Statements, stmt => stmt.Name == name));
        }

        private static Export Without(this Export export, string name)
        {
            Assert.DoesNotContain(export.Statements, stmt => stmt.Name == name);
            return export;
        }

        public static AssertedStatement WithSingle(this Export export)
        {
            return new(Assert.Single(export.Statements));
        }

        public static AssertedCommand WithCommand(this Export export, string name)
        {
            return export.With(name).Command(name);
        }

        public static AssertedQuery WithQuery(this Export export, string name)
        {
            return export.With(name).Query(name);
        }

        public static AssertedDto WithDto(this Export export, string name)
        {
            return export.With(name).Dto(name);
        }

        public static AssertedEnum WithEnum(this Export export, string name)
        {
            return export.With(name).Enum(name);
        }

        public static AssertedCommand Command(this AssertedStatement stmt, string name)
        {
            Assert.NotNull(stmt.Statement.Command);
            Assert.Equal(name, stmt.Statement.Name);
            return new(stmt.Statement);
        }

        public static AssertedQuery Query(this AssertedStatement stmt, string name)
        {
            Assert.NotNull(stmt.Statement.Query);
            Assert.Equal(name, stmt.Statement.Name);
            return new(stmt.Statement);
        }

        public static AssertedDto Dto(this AssertedStatement stmt, string name)
        {
            Assert.NotNull(stmt.Statement.Dto);
            Assert.Equal(name, stmt.Statement.Name);
            return new(stmt.Statement);
        }

        public static AssertedEnum Enum(this AssertedStatement stmt, string name)
        {
            Assert.NotNull(stmt.Statement.Enum);
            Assert.Equal(name, stmt.Statement.Name);
            return new(stmt.Statement);
        }

        public static AssertedStatement Commented(this AssertedStatement stmt, string comment)
        {
            Assert.Equal(comment, stmt.Statement.Comment);
            return stmt;
        }

        public static AssertedQuery WithReturnType(this AssertedQuery stmt, TypeRef typeRef)
        {
            Assert.Equal(typeRef, stmt.Statement.Query.ReturnType);
            return stmt;
        }

        public static AssertedCommand WithErrorCode(this AssertedCommand stmt, ErrorCode errorCode)
        {
            Assert.Contains(errorCode, stmt.Statement.Command.ErrorCodes);
            return stmt;
        }

        public static AssertedEnum WithMember(this AssertedEnum stmt, string name, long value, string comment = "")
        {
            var c = Assert.Single(stmt.Statement.Enum.Members, c => c.Name == name);
            Assert.Equal(value, c.Value);
            Assert.Equal(comment, c.Comment);
            return stmt;
        }

        public static T ThatExtends<T>(this T stmt, TypeRef typeRef)
            where T : AssertedStatement
        {
            Assert.Contains(typeRef, stmt.Statement.Extends);
            return stmt;
        }

        public static T WithConstant<T>(this T stmt, string name, object? value, string comment = "")
            where T : AssertedStatement
        {
            var c = Assert.Single(stmt.Statement.Constants, c => c.Name == name);
            Assert.Equal(value.ToValueRef(), c.Value);
            Assert.Equal(comment, c.Comment);
            return stmt;
        }

        public static T WithConstants<T>(this T stmt, int count)
            where T : AssertedStatement
        {
            Assert.Equal(count, stmt.Statement.Constants.Count);
            return stmt;
        }

        public static T WithNoConstants<T>(this T stmt)
            where T : AssertedStatement
        {
            return stmt.WithConstants(0);
        }

        public static T WithProperty<T>(this T stmt, string name, TypeRef type, string comment = "")
            where T : AssertedStatement
        {
            var prop = Assert.Single(stmt.Statement.Properties, p => p.Name == name);
            Assert.Equal(type, prop.Type);
            Assert.Equal(comment, prop.Comment);
            return stmt;
        }
    }

    public record AssertedStatement(Statement Statement);
    public record AssertedCommand(Statement Statement) : AssertedStatement(Statement);
    public record AssertedQuery(Statement Statement) : AssertedStatement(Statement);
    public record AssertedDto(Statement Statement) : AssertedStatement(Statement);
    public record AssertedEnum(Statement Statement) : AssertedStatement(Statement);
}
