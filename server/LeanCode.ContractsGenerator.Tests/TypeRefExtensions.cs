using System;

namespace LeanCode.ContractsGenerator.Tests
{
    public static class TypeRefExtensions
    {
        public static TypeRef Generic(string name)
        {
            return new()
            {
                Generic = new() { Name = name },
            };
        }

        public static TypeRef Known(KnownType type)
        {
            return new()
            {
                Known = new() { Type = type },
            };
        }

        public static TypeRef Internal(string name)
        {
            return new()
            {
                Internal = new() { Name = name },
            };
        }

        public static TypeRef Map(TypeRef key, TypeRef value)
        {
            var typeRef = Known(KnownType.Map);
            typeRef.Known.Arguments.Add(key);
            typeRef.Known.Arguments.Add(value);
            return typeRef;
        }

        public static TypeRef Array(TypeRef value)
        {
            var typeRef = Known(KnownType.Array);
            typeRef.Known.Arguments.Add(value);
            return typeRef;
        }

        public static TypeRef Nullable(this TypeRef typeRef)
        {
            typeRef.Nullable = true;
            return typeRef;
        }

        public static TypeRef WithArgument(this TypeRef typeRef, TypeRef arg)
        {
            if (typeRef.Internal is not null)
            {
                typeRef.Internal.Arguments.Add(arg);
            }
            else if (typeRef.Known is not null)
            {
                typeRef.Known.Arguments.Add(arg);
            }
            else
            {
                throw new ArgumentException("Cannot add generic arguments to other type than `Internal` and `Known`.");
            }

            return typeRef;
        }
    }
}
