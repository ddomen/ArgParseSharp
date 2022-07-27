namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UIntAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(uint);
        public UIntAttribute(uint constant) : base(constant) { }
        public UIntAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new UInt(name, (UInt.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new UInt(name, value.As<uint>());
    }
}