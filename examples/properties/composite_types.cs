using System;
using System.Collections;
using System.Collections.Generic;
using LeanCode.CQRS;
public class Dto
{
    public IReadOnlyList<int> A { get; set; }
    public IList<int> B { get; set; }
    public List<int> C { get; set; }
    public int[] D { get; set; }
    public IReadOnlyDictionary<int, string> E { get; set; }
    public IDictionary<int, string> F { get; set; }
    public Dictionary<int, string> G { get; set; }
}
