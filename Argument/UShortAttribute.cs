namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UShortAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(ushort);
        public UShortAttribute(ushort constant) : base(constant) { }
        public UShortAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new UShort(name, (UShort.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new UShort(name, value.As<ushort>());
    }
}