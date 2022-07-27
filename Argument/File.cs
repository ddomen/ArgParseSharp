namespace ArgParseSharp;

public partial class Argument {
    public class File : Argument<FileInfo> {
        public File(string name, FileInfo? constant) : base(name, constant) => IgnoredPrefix = "@";
        public File(string name, ParserDelegate? type = null) : base(name, type ?? DefaultParser) => IgnoredPrefix = "@";
        public static FileInfo DefaultParser(string value) => new(value);
    }
}
