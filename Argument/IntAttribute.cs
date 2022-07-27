namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IntAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(int);
        protected override T _delegateType => typeof(Int.ParserDelegate);
        public IntAttribute() : base(Int.DefaultParser) { }
        public IntAttribute(int constant) : base(constant) { }
        public IntAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Int(name, (Int.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Int(name, value.As<int>());
    }
}