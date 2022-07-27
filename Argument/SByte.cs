namespace ArgParseSharp;

public partial class Argument {
    public class SByte : Argument<sbyte> {
        public SByte(string name, sbyte constant) : base(name, constant) { }
        public SByte(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static sbyte DefaultParser(string value) => sbyte.TryParse(value, out sbyte r) ? r : (sbyte)0;
    }
}
