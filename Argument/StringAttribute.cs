namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(string);
        protected override T _delegateType => typeof(String.ParserDelegate);
        public StringAttribute() : base(String.DefaultParser) { }
        public StringAttribute(string? constant) : base(constant) { }
        public StringAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new String(name, (String.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new String(name, value.As<string>());
    }
}