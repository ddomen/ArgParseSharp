namespace ArgParseSharp;

public partial class Argument {
    public class Byte : Argument<byte> {
        public Byte(string name, byte constant) : base(name, constant) { }
        public Byte(string name, Parser? parser = null) : base(name, parser ?? DefaultParser) { }
        public static byte DefaultParser(string value) => byte.TryParse(value, out byte r) ? r : byte.MinValue;
    }
}
