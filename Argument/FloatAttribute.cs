namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FloatAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(float);
        protected override T _delegateType => typeof(Float.ParserDelegate);
        public FloatAttribute() : base(Float.DefaultParser) { }
        public FloatAttribute(float constant) : base(constant) { }
        public FloatAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Float(name, (Float.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Float(name, value.As<float>());
    }
}