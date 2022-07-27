﻿namespace ArgParseSharp;

public partial class Argument {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ShortAttribute : ArgumentAttribtue {
        protected override Type _resultType => typeof(short);
        public ShortAttribute(short constant) : base(constant) { }
        public ShortAttribute(Type parserOwner, string? parserName = null) : base(parserOwner, parserName) { }
        internal override Argument CreateArgument(string name, Delegate? type) => new Short(name, (Short.Parser?)type);
        internal override Argument CreateArgument(string name, Nullable<object> value) => new Short(name, value.As<short>());
    }
}