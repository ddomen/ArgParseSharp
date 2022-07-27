namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DoubleAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(double);
        public DoubleAttribute(double constant) : base(constant) { }
        public DoubleAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new Double(name, (Double.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new Double(name, value.As<double>());
    }
}