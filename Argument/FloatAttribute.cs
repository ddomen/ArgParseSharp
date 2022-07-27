namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FloatAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(float);
        protected override Type _delegateType => typeof(Float.Parser);
        public FloatAttribute() : base(Float.DefaultParser) { }
        public FloatAttribute(float constant) : base(constant) { }
        public FloatAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Float(name, (Float.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Float(name, value.As<float>());
    }
}