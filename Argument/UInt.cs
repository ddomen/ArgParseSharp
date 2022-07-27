namespace ArgParseSharp;

public partial class Argument {
    public class UInt : Argument<uint> {
        public UInt(string name, uint constant) : base(name, constant) { }
        public UInt(string name, Parser? parser = null) : base(name, parser ?? DefaultParser) { }
        public static uint DefaultParser(string value) => uint.TryParse(value, out uint r) ? r : 0U;
    }
}
