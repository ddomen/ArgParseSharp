namespace ArgParseSharp;

public partial class Argument {
    public class Char : Argument<char> {
        public Char(string name, char constant) : base(name, constant) { }
        public Char(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static char DefaultParser(string value) => value.Length > 0 ? value[0] : char.MinValue;
    }
}
