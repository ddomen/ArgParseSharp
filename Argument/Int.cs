namespace ArgParseSharp;

public partial class Argument {
    public class Int : Argument<int> {
        public Int(string name, int constant) : base(name, constant) { }
        public Int(string name, Parser? parser = null) : base(name, parser ?? DefaultParser) { }
        public static int DefaultParser(string value) => int.TryParse(value, out int r) ? r : 0;
    }
}
