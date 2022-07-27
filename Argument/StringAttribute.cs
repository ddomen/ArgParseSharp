namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StringAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(string);
        public StringAttribute(string? constant) : base(constant) { }
        public StringAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new String(name, (String.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new String(name, value.As<string>());
    }
}