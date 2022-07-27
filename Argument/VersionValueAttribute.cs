namespace ArgParseSharp;

using T = Type;
using V = Version;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VersionValueAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(V);
        protected override T _delegateType => typeof(VersionValue.ParserDelegate);
        public VersionValueAttribute() : base(VersionValue.DefaultParser) { }
        public VersionValueAttribute(V constant) : base(constant) { }
        public VersionValueAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new VersionValue(name, (VersionValue.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new VersionValue(name, value.As<V>());
    }
}