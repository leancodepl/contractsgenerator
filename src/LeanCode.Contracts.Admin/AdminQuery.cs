namespace LeanCode.Contracts.Admin;

public abstract class AdminQuery<TResult> : IQuery<AdminQueryResult<TResult>>
{
    /// <remarks>0-based</remarks>
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
public class AdminFilterForAttribute(string name) : Attribute;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class AdminLabelAttribute(string label) : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AdminColumnAttribute(string? name) : Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class AdminSortableAttribute : Attribute;
