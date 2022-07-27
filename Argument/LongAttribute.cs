namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LongAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(long);
        protected override T _delegateType => typeof(Long.ParserDelegate);
        public LongAttribute() : base(Long.DefaultParser) { }
        public LongAttribute(long constant) : base(constant) { }
        public LongAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Long(name, (Long.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Long(name, value.As<long>());
    }
}