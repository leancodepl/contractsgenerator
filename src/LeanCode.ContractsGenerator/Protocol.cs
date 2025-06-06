using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace LeanCode.ContractsGenerator;

public readonly record struct ProtocolVersion(uint Major, uint Minor = 0)
    : IComparable<ProtocolVersion>,
        IComparisonOperators<ProtocolVersion, ProtocolVersion, bool>,
        IParsable<ProtocolVersion>
{
    public static readonly ProtocolVersion Current = new(1);

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

public partial class Protocol
{
    public static readonly string CurrentVersion = ProtocolVersion.Current.ToString();

    public static class KnownExtensions
    {
        public const string DateTime = "datetime";
    }

    public Protocol(GeneratorConfiguration configuration)
    {
        Version = CurrentVersion;

        if (configuration.AllowDateTime)
        {
            Extensions.Add(KnownExtensions.DateTime);
        }
    }

    public static Protocol Max(IEnumerable<Protocol> items)
    {
        items = items.Where(p => p is not null);

        var versions = items
            .Where(p => !string.IsNullOrEmpty(p.Version))
            .Select(p => ProtocolVersion.Parse(p.Version, null))
            .ToList();

        if (versions.FirstOrDefault() is { } ver && versions.Any(pv => pv.Major != ver.Major))
        {
            throw new InvalidOperationException("Items must have a single major version.");
        }

        var version = versions.DefaultIfEmpty(ProtocolVersion.Current).Max();
        var extensions = items.SelectMany(p => p.Extensions).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        return new() { Version = version.ToString(), Extensions = { { extensions } } };
    }
}
