namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LongAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(long);
        protected override Type _delegateType => typeof(Long.Parser);
        public LongAttribute() : base(Long.DefaultParser) { }
        public LongAttribute(long constant) : base(constant) { }
        public LongAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Long(name, (Long.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Long(name, value.As<long>());
    }
}