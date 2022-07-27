namespace ArgParseSharp;

public partial class Argument {
    public class Long : Argument<long> {
        public Long(string name, long constant) : base(name, constant) { }
        public Long(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static long DefaultParser(string value) => long.TryParse(value, out long r) ? r : 0L;
    }
}
