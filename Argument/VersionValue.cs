namespace ArgParseSharp;

using V = System.Version;

public partial class Argument {
    public class VersionValue : Argument<V> {
        public VersionValue(string name, V? constant) : base(name, constant) { }
        public VersionValue(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static V DefaultParser(string value) => V.TryParse(value, out V r) ? r : new(0, 0, 0, 0);
    }
}
