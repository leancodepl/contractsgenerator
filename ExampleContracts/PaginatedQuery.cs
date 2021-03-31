using System.Collections.Generic;
using LeanCode.CQRS;

namespace LeanCode.ContractsGeneratorV2.ExampleContracts
{
    /// This is a class-level comment.
    public abstract class PaginatedQuery<TResult> : IRemoteQuery<PaginatedResult<TResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// This one is in XML.
    /// </summary>
    public class PaginatedResult<TResult>
    {
        /// And this is a property comment.
        public List<TResult> Items { get; set; }
        public long TotalCount { get; set; }
    }
}