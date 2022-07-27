namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StringAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(string);
        protected override Type _delegateType => typeof(String.Parser);
        public StringAttribute() : base(String.DefaultParser) { }
        public StringAttribute(string? constant) : base(constant) { }
        public StringAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new String(name, (String.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new String(name, value.As<string>());
    }
}