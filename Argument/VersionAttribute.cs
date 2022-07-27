namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class VersionAttribute : BooleanAttribute {
        public VersionAttribute() : base(true) {
            Name = Version.DEFAULT_NAME;
            Flags = new string[] { "-v" };
            ArgNumber = 0;
        }
    }
}