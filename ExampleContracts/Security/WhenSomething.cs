using System;
using System.Diagnostics.CodeAnalysis;
using LeanCode.CQRS.Security;

namespace LeanCode.ContractsGeneratorV2.ExampleContracts.Security
{
    public interface ISomethingRelated
    {
        public Guid SomethingId { get; set; }
    }

    [SuppressMessage("?", "SA1302", Justification = "Convention for authorizers.")]
    [SuppressMessage("?", "IDE1006", Justification = "Convention for authorizers.")]
    public interface WhenHasSomethingAccess { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AuthorizeWhenHasSomethingAccessAttribute : AuthorizeWhenAttribute
    {
        public AuthorizeWhenHasSomethingAccessAttribute()
            : base(typeof(WhenHasSomethingAccess))
        { }
    }
}