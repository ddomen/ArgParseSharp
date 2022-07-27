namespace ArgParseSharp;

public partial class Argument {
    public class Short : Argument<short> {
        public Short(string name, short constant) : base(name, constant) { }
        public Short(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static short DefaultParser(string value) => short.TryParse(value, out short r) ? r : (short)0;
    }
}
