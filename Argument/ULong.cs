namespace ArgParseSharp;

public partial class Argument {
    public class ULong : Argument<ulong> {
        public ULong(string name, ulong constant) : base(name, constant) { }
        public ULong(string name, Parser? parser = null) : base(name, parser ?? DefaultParser) { }
        public static ulong DefaultParser(string value) => ulong.TryParse(value, out ulong r) ? r : 0UL;
    }
}
