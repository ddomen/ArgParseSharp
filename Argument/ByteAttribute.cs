namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ByteAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(byte);
        protected override T _delegateType => typeof(Byte.ParserDelegate);
        public ByteAttribute() : base(Byte.DefaultParser) { }
        public ByteAttribute(byte constant) : base(constant) { }
        public ByteAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Byte(name, (Byte.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Byte(name, value.As<byte>());
    }
}