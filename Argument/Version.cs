namespace ArgParseSharp;

public partial class Argument {
    public class Version : Boolean {
        public Version(string? name = null) : base(name ?? DEFAULT_NAME, true) => Flags = new string[] { "version", "v" };
        public const string DEFAULT_NAME = "Version";
    }
}
