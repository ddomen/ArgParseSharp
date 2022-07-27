namespace ArgParseSharp;

public partial class Argument {
    public class Boolean : Argument<bool> {
        public Boolean(string name) : this(name, DefaultParser) { }
        public Boolean(string name, bool constant) : base(name, constant) { ArgNumber = 0; }
        public Boolean(string name, Parser? parser) : base(name, parser ?? DefaultParser) { ArgNumber = 0; }
        public static bool DefaultParser(string value) => value.ToLower() is not ("false" or "0");
    }
}
