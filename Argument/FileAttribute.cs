namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FileAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(FileInfo);
        public FileAttribute(string? constant) : base(constant) { }
        public FileAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new File(name, (File.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new File(name, value.As<FileInfo>());
    }
}