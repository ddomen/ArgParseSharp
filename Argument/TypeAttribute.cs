namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TypeAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(T);
        protected override T _delegateType => typeof(Type.ParserDelegate);
        public TypeAttribute() : base(Type.DefaultParser) { }
        public TypeAttribute(string? constant) : base(constant) { }
        public TypeAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new Type(name, (Type.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new Type(name, value.As<T>());
    }
}