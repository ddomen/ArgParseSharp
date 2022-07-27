using System.Reflection;

namespace ArgParseSharp;

public partial class Argument {
    public class String : Argument<string> {
        public String(string name, string? constant) : base(name, constant ?? string.Empty) { }
        public String(string name, Parser? type = null) : base(name, type) { }
        protected override string? ResolveChecked(string value) =>
            _type is null ? value : (string?)_type.DynamicInvoke(value);
        protected override void CheckDelegate(MethodInfo? method) {
            if (method is null) { return; }
            base.CheckDelegate(method);
        }
        public static string DefaultParser(string value) => value;
    }
}
