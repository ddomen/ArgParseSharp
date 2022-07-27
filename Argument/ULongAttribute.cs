namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ULongAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(ulong);
        public ULongAttribute(ulong constant) : base(constant) { }
        public ULongAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new ULong(name, (ULong.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new ULong(name, value.As<ulong>());
    }
}