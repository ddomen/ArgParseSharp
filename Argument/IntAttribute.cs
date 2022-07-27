namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IntAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(int);
        public IntAttribute(int constant) : base(constant) { }
        public IntAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new Int(name, (Int.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new Int(name, value.As<int>());
    }
}