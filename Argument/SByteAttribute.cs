namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SByteAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(sbyte);
        public SByteAttribute(sbyte constant) : base(constant) { }
        public SByteAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new SByte(name, (SByte.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new SByte(name, value.As<sbyte>());
    }
}