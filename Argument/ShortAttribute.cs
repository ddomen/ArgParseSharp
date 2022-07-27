namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShortAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(short);
        protected override T _delegateType => typeof(Short.ParserDelegate);
        public ShortAttribute() : base(Short.DefaultParser) { }
        public ShortAttribute(short constant) : base(constant) { }
        public ShortAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Short(name, (Short.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Short(name, value.As<short>());
    }
}