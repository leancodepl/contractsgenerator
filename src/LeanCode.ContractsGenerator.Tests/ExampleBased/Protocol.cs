using Xunit;

namespace LeanCode.ContractsGenerator.Tests.ExampleBased;

public class Protocol
{
    [Fact]
    public void Emits_current_protocol_version_with_no_extensions_by_default()
    {
        "simple/dto.cs"
            .Compiles()
            .WithProtocolVersion(ContractsGenerator.Protocol.CurrentVersionAsString)
            .WithProtocolExtensions([]);
    }

    [Fact]
    public void Emits_current_protocol_version_with_datetime_extension_if_datetime_is_allowed()
    {
        "simple/dto.cs"
            .Compiles(new(AllowDateTime: true))
            .WithProtocolVersion(ContractsGenerator.Protocol.CurrentVersionAsString)
            .WithProtocolExtension(ContractsGenerator.Protocol.KnownExtensions.DateTime);
    }
}
