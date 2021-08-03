public interface A { public int PropA { get; set; } }
public class B { public int PropB { get; set; } }
public class C : B, A { public int PropA { get; set; } public int PropC { get; set; } }
