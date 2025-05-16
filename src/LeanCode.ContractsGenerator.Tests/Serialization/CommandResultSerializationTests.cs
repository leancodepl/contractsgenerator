using System.Text.Json;
using FluentAssertions;
using LeanCode.Contracts;
using LeanCode.Contracts.Validation;
using Xunit;

namespace LeanCode.ContractsGenerator.Tests.Serialization;

public class CommandResultSerializationTests
{
    private static readonly CommandResult SampleCommandResult = CommandResult.NotValid(
        new([new ValidationError("A property", "An error message", 1)])
    );

    private const string Json = $$"""
        {
          "{{nameof(CommandResult.ValidationErrors)}}": [
            {
              "{{nameof(ValidationError.PropertyName)}}": "A property",
              "{{nameof(ValidationError.ErrorMessage)}}": "An error message",
              "{{nameof(ValidationError.ErrorCode)}}": 1
            }
          ],
          "{{nameof(CommandResult.WasSuccessful)}}": false
        }
        """;

    [Fact]
    public void CommandResult_is_serializable()
    {
        var serialized = JsonSerializer.Serialize(
            SampleCommandResult,
            new JsonSerializerOptions { WriteIndented = true }
        );

        serialized.Should().Be(Json);
    }

    [Fact]
    public void CommandResult_is_deserializable()
    {
        var deserialized = JsonSerializer.Deserialize<CommandResult>(Json);

        deserialized.Should().BeEquivalentTo(SampleCommandResult);
    }
}
