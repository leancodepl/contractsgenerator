namespace LeanCode.ContractsGenerator.Tests;

public static class ErrorCodeExtensions
{
    public static ErrorCode Single(string name, int value)
    {
        return new ErrorCode
        {
            Single = new()
            {
                Name = name,
                Code = value,
            },
        };
    }

    public static ErrorCode Group(string name, string groupId, params ErrorCode[] inner)
    {
        var g = new ErrorCode
        {
            Group = new()
            {
                Name = name,
                GroupId = groupId,
            },
        };
        g.Group.InnerCodes.AddRange(inner);
        return g;
    }
}
