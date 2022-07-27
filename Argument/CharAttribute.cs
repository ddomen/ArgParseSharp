namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CharAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(char);
        protected override T _delegateType => typeof(Char.ParserDelegate);
        public CharAttribute() : base(Char.DefaultParser) { }
        public CharAttribute(byte constant) : base(constant) { }
        public CharAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Char(name, (Char.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Char(name, value.As<char>());
    }
}