namespace LeanCode.Contracts.Admin;

public abstract class AdminQuery<TResult> : IQuery<AdminQueryResult<TResult>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }

    public bool? SortDescending { get; set; }
    public string? SortBy { get; set; }
}

public class AdminQueryResult<TResult>
{
    public long Total { get; set; }
    public List<TResult> Items { get; set; }
}

public class AdminFilterRange<T>
{
    public T? From { get; set; }
    public T? To { get; set; }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AdminFilterFor : Attribute
{
    public AdminFilterFor(string name) { }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class AdminLabel : Attribute
{
    public AdminLabel(string label) { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AdminColumn : Attribute
{
    public AdminColumn(string? name) { }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AdminSortable : Attribute
{
    public AdminSortable() { }
}
