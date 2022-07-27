namespace ArgParseSharp;

using T = Type;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileAttribute : ArgumentAttribute {
        protected override T _resultType => typeof(FileInfo);
        protected override T _delegateType => typeof(File.ParserDelegate);
        public FileAttribute() : base(File.DefaultParser) { }
        public FileAttribute(string? constant) : base(constant) { }
        public FileAttribute(T parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        protected override Argument CreateArgument(string name, Delegate? type) => new File(name, (File.ParserDelegate?)type);
        protected override Argument CreateArgument(string name, Nullable<object> value) => new File(name, value.As<FileInfo>());
    }
}