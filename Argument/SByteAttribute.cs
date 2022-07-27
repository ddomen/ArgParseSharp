namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SByteAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(sbyte);
        protected override Type _delegateType => typeof(SByte.Parser);
        public SByteAttribute() : base(SByte.DefaultParser) { }
        public SByteAttribute(sbyte constant) : base(constant) { }
        public SByteAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new SByte(name, (SByte.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new SByte(name, value.As<sbyte>());
    }
}