namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BooleanAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(bool);
        protected override T _delegateType => typeof(Boolean.ParserDelegate);
        public BooleanAttribute(bool constant = true) : base(constant) { ArgNumber = 0; }
        public BooleanAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { ArgNumber = 0; }
        protected override Argument CreateArgument(string name, Delegate? type) => new Boolean(name, (Boolean.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Boolean(name, value.As<bool>());
    }
}