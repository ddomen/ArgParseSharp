namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SByteAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(sbyte);
        protected override T _delegateType => typeof(SByte.ParserDelegate);
        public SByteAttribute() : base(SByte.DefaultParser) { }
        public SByteAttribute(sbyte constant) : base(constant) { }
        public SByteAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new SByte(name, (SByte.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new SByte(name, value.As<sbyte>());
    }
}