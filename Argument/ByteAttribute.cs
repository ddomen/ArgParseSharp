﻿namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ByteAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(byte);
        public ByteAttribute(byte constant) : base(constant) { }
        public ByteAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new Byte(name, (Byte.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new Byte(name, value.As<byte>());
    }
}