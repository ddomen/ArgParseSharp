namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoubleAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(double);
        protected override T _delegateType => typeof(Double.ParserDelegate);
        public DoubleAttribute() : base(Double.DefaultParser) { }
        public DoubleAttribute(double constant) : base(constant) { }
        public DoubleAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Double(name, (Double.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Double(name, value.As<double>());
    }
}