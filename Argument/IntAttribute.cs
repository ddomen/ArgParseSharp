namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IntAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(int);
        protected override Type _delegateType => typeof(Int.Parser);
        public IntAttribute() : base(Int.DefaultParser) { }
        public IntAttribute(int constant) : base(constant) { }
        public IntAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Int(name, (Int.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Int(name, value.As<int>());
    }
}