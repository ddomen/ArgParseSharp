namespace ArgParseSharp;

public partial class Argument {
    public class Version : Boolean {
        public Version(string name = DEFAULT_NAME) : base(name, true) => Flags = new string[] { "-v" };
        public const string DEFAULT_NAME = "--version";
    }
}
