namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UIntAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(uint);
        protected override T _delegateType => typeof(UInt.ParserDelegate);
        public UIntAttribute() : base(UInt.DefaultParser) { }
        public UIntAttribute(uint constant) : base(constant) { }
        public UIntAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new UInt(name, (UInt.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new UInt(name, value.As<uint>());
    }
}