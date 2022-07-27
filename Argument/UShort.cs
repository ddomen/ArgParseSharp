namespace ArgParseSharp;

public partial class Argument {
    public class UShort : Argument<ushort> {
        public UShort(string name, ushort constant) : base(name, constant) { }
        public UShort(string name, ParserDelegate? parser = null) : base(name, parser ?? DefaultParser) { }
        public static ushort DefaultParser(string value) => ushort.TryParse(value, out ushort r) ? r : ushort.MinValue;
    }
}
