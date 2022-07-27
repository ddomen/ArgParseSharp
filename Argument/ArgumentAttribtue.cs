using System.Reflection;

namespace ArgParseSharp;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ArgumentAttribtue : Attribute {
    protected Delegate? _type;
    protected Nullable<object> _constant;
    public string? Name { get; set; }
    public string[]? Flags { get; set; }
    public NArgs? ArgNumber { get; set; }
    public Nullable<object> Constant {
        get => _constant;
        set => SetConstant(value);
    }
    public Nullable<object> Default { get; set; }
    public Delegate? Type {
        get => _type;
        set => SetType(_type);
    }
    public string[]? Choiches { get; set; }
    public bool Required { get; set; }
    public string? Help { get; set; }
    public string?[]? MetaVariables { get; set; }
    public string? MemberName { get; set; }
    public string? Group { get; set; }
    public string[] GroupHierarchy {
        get => (Group ?? string.Empty).Split('.').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        set => Group = string.Join(".", (value ?? Array.Empty<string>()));
    }
    public string? IgnoredPrefix { get; set; }
    public string? IgnoredSuffix { get; set; }

    public IEnumerable<string> Identifiers =>
        Name is null ? (Flags ?? Enumerable.Empty<string>()) :
        (Flags ?? Enumerable.Empty<string>()).Prepend(Name);

    protected virtual Type _resultType => typeof(object);
    protected virtual Type _delegateType => typeof(Argument.Parser);

    public ArgumentAttribtue(object? constant) => Constant = constant;
    public ArgumentAttribtue(Type parserOwner, string? parserName = null) => SetType(parserOwner, parserName);
    protected ArgumentAttribtue(Delegate? type) => SetType(type);
    protected void CheckDelegate(MethodInfo? method) {
        if (method is null) { throw new ArgumentNullException("type"); }
        if (!CheckReturn(method, _resultType)) { throw new ArgumentException($"Expected parser to return {_resultType} but it returned {method.ReturnType}"); }
        if (!CheckParameters(method, typeof(string))) { throw new ArgumentException("Type must have a string parameter"); }
    }

    public void SetType(Type owner, string? name = null) {
        name = (name ?? DEFAULT_PARSER_NAME).ToLower();
        MemberInfo[] ms = owner.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (MemberInfo m in ms) {
            if (m.Name.ToLower() != name) { continue; }
            if (m is MethodInfo mi) { try { SetType(mi.CreateDelegate(typeof(Delegate))); return; } catch { } }
            else if (m is FieldInfo fi && typeof(Delegate).IsAssignableFrom(fi.DeclaringType)) {
                try { SetType((Delegate)fi.GetValue(null)); return; } catch { }
            }
            else if (m is PropertyInfo pi && typeof(Delegate).IsAssignableFrom(pi.DeclaringType)) {
                try { SetType((Delegate)pi.GetValue(null)); return; } catch { }
            }
        }
        throw new ArgumentException($"Parser not found: static object {owner}.{name}(string)");
    }

    internal virtual Argument SetArgProps(Argument target, MemberInfo? member = null) {
        target.ArgNumber = ArgNumber;
        target.Choiches = Choiches;
        target.Default = Default;
        target.Flags = Flags;
        target.Group = Group;
        target.Help = Help;
        target.IgnoredPrefix = IgnoredPrefix;
        target.IgnoredSuffix = IgnoredSuffix;
        target.MemberName = MemberName ?? member?.Name;
        target.MetaVariables = MetaVariables;
        target.Required = Required;
        return target;
    }
    internal Argument CreateArgument(MemberInfo member) =>
        SetArgProps(CreateArgument(Name ?? member.Name), member);
    internal Argument CreateArgument(string name) =>
        Constant.HasValue ?
            CreateArgument(name, Constant) :
            CreateArgument(name, MakeDelegate(Type));
    protected virtual Argument CreateArgument(string name, Nullable<object> constant) => new(name, constant);
    protected virtual Argument CreateArgument(string name, Delegate? type) => new(name, type);
    protected virtual Delegate? MakeDelegate(Delegate? target) =>
        target is null ? null :
        _delegateType.IsInstanceOfType(target) ? target :
        target.Method.IsStatic ?
        Delegate.CreateDelegate(_delegateType, target.Method) :
        Delegate.CreateDelegate(_delegateType, target.Target, target.Method);

    public void SetType(Delegate? type) {
        CheckDelegate(type?.Method);
        _type = type;
    }
    public void SetConstant(Nullable<object> constant) {
        if (!constant.HasValue) { CheckDelegate(_type?.Method); }
        _constant = constant;
    }

    protected static bool CheckReturn(MethodInfo? method, Type type) =>
        method is not null && method.ReturnType == type;
    protected static bool CheckParameters(MethodInfo? method, params Type[] parameters) {
        if (method is null) { return false; }
        parameters ??= System.Type.EmptyTypes;
        Type[] args = method.GetParameters().Select(p => p.ParameterType).ToArray();
        return args.Length == parameters.Length &&
                args.Zip(parameters, (a, p) => p.IsAssignableFrom(a)).All(b => b);
    }

    public const string DEFAULT_PARSER_NAME = "DefaultParser";
}
