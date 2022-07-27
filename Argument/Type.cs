namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    public class Type : Argument<T> {
        public Type(string name, T? constant) : base(name, constant) { }
        public Type(string name, ParserDelegate? type = null) : base(name, type ?? DefaultParser) { }
        public static T? DefaultParser(string value) => T.GetType(value, false, true);
    }
}
