using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace LeanCode.ContractsGenerator;

public readonly record struct ProtocolVersion(uint Major, uint Minor = 0)
    : IComparable<ProtocolVersion>,
        IComparisonOperators<ProtocolVersion, ProtocolVersion, bool>,
        IParsable<ProtocolVersion>
{
    public static ProtocolVersion Parse(string s, IFormatProvider? provider)
    {
        var dot = s.IndexOf('.');

        if (dot == -1)
        {
            return new(uint.Parse(s, provider));
        }

        var major = s.AsSpan(..dot);
        var minor = s.AsSpan((dot + 1)..);

        return new(uint.Parse(major), uint.Parse(minor));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ProtocolVersion result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var dot = s.IndexOf('.');

        if (dot == -1)
        {
            if (uint.TryParse(s, provider, out var version))
            {
                result = new(version);
                return true;
            }
        }
        else
        {
            if (
                uint.TryParse(s.AsSpan(..dot), provider, out var major)
                && uint.TryParse(s.AsSpan((dot + 1)..), provider, out var minor)
            )
            {
                result = new(major, minor);
                return true;
            }
        }

        result = default;
        return false;
    }

    public int CompareTo(ProtocolVersion other)
    {
        var major = Major.CompareTo(other.Major);
        return major != 0 ? major : Minor.CompareTo(other.Minor);
    }

    public override string ToString() => Minor == 0u ? Major.ToString() : $"{Major}.{Minor}";

    public static bool operator <(ProtocolVersion left, ProtocolVersion right) => left.CompareTo(right) < 0;

    public static bool operator >(ProtocolVersion left, ProtocolVersion right) => left.CompareTo(right) > 0;

    public static bool operator <=(ProtocolVersion left, ProtocolVersion right) => left.CompareTo(right) <= 0;

    public static bool operator >=(ProtocolVersion left, ProtocolVersion right) => left.CompareTo(right) >= 0;
}
