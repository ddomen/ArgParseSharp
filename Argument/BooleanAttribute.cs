namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BooleanAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(bool);
        public BooleanAttribute(bool constant = true) : base(constant) { ArgNumber = 0; }
        public BooleanAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { ArgNumber = 0; }
        internal override Argument CreateArgument(string name, Delegate? type) => new Boolean(name, (Boolean.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new Boolean(name, value.As<bool>());
    }
}