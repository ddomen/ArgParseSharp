namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ULongAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(ulong);
        protected override T _delegateType => typeof(ULong.ParserDelegate);
        public ULongAttribute() : base(ULong.DefaultParser) { }
        public ULongAttribute(ulong constant) : base(constant) { }
        public ULongAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new ULong(name, (ULong.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new ULong(name, value.As<ulong>());
    }
}