namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CharAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(char);
        protected override Type _delegateType => typeof(Char.Parser);
        public CharAttribute() : base(Char.DefaultParser) { }
        public CharAttribute(byte constant) : base(constant) { }
        public CharAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Char(name, (Char.Parser?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Char(name, value.As<char>());
    }
}