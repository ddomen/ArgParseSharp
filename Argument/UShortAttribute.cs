namespace ArgParseSharp;

using T = Type;
public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UShortAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(ushort);
        protected override T _delegateType => typeof(UShort.ParserDelegate);
        public UShortAttribute() : base(UShort.DefaultParser) { }
        public UShortAttribute(ushort constant) : base(constant) { }
        public UShortAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new UShort(name, (UShort.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new UShort(name, value.As<ushort>());
    }
}