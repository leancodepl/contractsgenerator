namespace LeanCode.ContractsGenerator;

public partial class Protocol
{
    public Protocol(GeneratorConfiguration configuration)
    {
        Version = CurrentVersionAsString;

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

        var version = versions.DefaultIfEmpty(CurrentVersion).Max();
        var extensions = items.SelectMany(p => p.Extensions).ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        return new() { Version = version.ToString(), Extensions = { { extensions } } };
    }
}
