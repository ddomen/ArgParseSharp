using System.Collections;
using System.Reflection;

namespace ArgParseSharp;

public partial class Argument {
    protected string _name;
    protected Delegate? _type;
    protected Nullable<object> _constant;
    protected Nullable<object> _default;
    public string Name {
        get => _name;
        set => _name = value ?? throw new ArgumentNullException(nameof(value));
    }
    public string[]? Flags { get; set; }
    public NArgs? ArgNumber { get; set; }
    public Nullable<object> Constant {
        get => _constant;
        set => SetConstant(value);
    }
    public Nullable<object> Default {
        get => _default;
        set => _default = value;
    }
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

    public IEnumerable<string> Identifiers => (Flags ?? Enumerable.Empty<string>()).Prepend(_name);

    public Argument(string name, object? constant) {
        _name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        Constant = constant;
    }
    public Argument(string name, Parser? parser) : this(name, (Delegate?)parser) { }
    public Argument(string name, Delegate? parser) {
        _name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        SetType(parser);
    }

    public object? Parse(string value) => Resolve(Ignore(value));

    public delegate object? Parser(string value);

    internal static bool CheckReturn(MethodInfo? method, Type type) => method is not null && method.ReturnType == type;
    internal static bool CheckReturnVoid(MethodInfo? method) => method is not null && method.ReturnType == typeof(void);
    internal static bool CheckParameters(MethodInfo? method, params Type[] parameters) {
        if (method is null) { return false; }
        parameters ??= System.Type.EmptyTypes;
        Type[] args = method.GetParameters().Select(p => p.ParameterType).ToArray();
        return args.Length == parameters.Length &&
                args.Zip(parameters, (a, p) => p.IsAssignableFrom(a)).All(b => b);
    }
    internal static bool StartsWith(string target, string prefixChars) => (target?.Length ?? 0) > 0 && prefixChars.Any(p => target![0] == p);

    internal string Ignore(string value) {
        if (IgnoredPrefix is not null && value.StartsWith(IgnoredPrefix)) { value = value.Substring(IgnoredPrefix.Length); }
        if (IgnoredSuffix is not null && value.EndsWith(IgnoredSuffix)) { value = value.Substring(0, value.Length - IgnoredSuffix.Length); }
        return value;
    }

    protected virtual object? Resolve(string value) =>
        Constant.HasValue ? Constant.Value :
        (_type ?? throw new NullReferenceException(nameof(Type))).DynamicInvoke(value);

    internal virtual void CheckDelegate(MethodInfo? method) {
        if (Constant.HasValue) { return; }
        if (method is null) { throw new ArgumentNullException("type"); }
        if (CheckReturnVoid(method)) { throw new ArgumentException("Type must return a value"); }
        if (!CheckParameters(method, typeof(string))) { throw new ArgumentException("Type must have a string parameter"); }
    }

    public void SetType(Delegate? type) {
        CheckDelegate(type?.Method);
        _type = type;
    }

    public void SetConstant(Nullable<object> constant) {
        if (!constant.HasValue) { CheckDelegate(_type?.Method); }
        _constant = constant;
    }
    internal bool IsPositional(string prefixChars) => !StartsWith(Name, prefixChars);
    internal bool Contains(string arg, char[] prefixes, bool ignoreCase) => Identifiers.Any(i => (ignoreCase ? i.ToLower() : i).TrimStart(prefixes) == arg.TrimStart(prefixes));
    internal bool Contains(string arg, string prefixes, bool ignoreCase) => Contains(arg, prefixes.ToCharArray(), ignoreCase);

    internal bool TryResolve(string[] arguments, ref int index, out object? value, string prefixes, bool ignoreCase) {
        value = null;
        bool isPositional = IsPositional(prefixes);
        string arg = arguments[index];
        if (!isPositional && !StartsWith(arg, prefixes)) { return false; }
        if (ignoreCase) { arg = arg.ToLower(); }
        if (!Contains(arg, prefixes, ignoreCase)) { return false; }
        NArgs nargs = ArgNumber ?? NArgs.ONE;
        switch (nargs.Value) {
            case NArgs.VALUE_ZERO: {
                index++;
                value = Parse(string.Empty);
                break;
            }
            case NArgs.VALUE_OPTIONAL: {
                index++;
                if (arguments.Length >= index + 1) { value = Parse(arguments[index++]); }
                break;
            }
            case NArgs.VALUE_ZERO_OR_MORE: {
                index++;
                List<object?> values = new();
                while (index < arguments.Length && !prefixes.Contains((arg = arguments[index])[0])) {
                    values.Add(Parse(arg));
                    index++;
                }
                value = values.ToArray();
                break;
            }
            case NArgs.VALUE_ONE_OR_MORE: {
                if (index + 1 > arguments.Length) { throw new ArgumentException("Not enough values"); }
                index++;
                List<object?> values = new() { Parse(arguments[index++]) };
                while (index < arguments.Length && !prefixes.Contains((arg = arguments[index])[0])) {
                    values.Add(Parse(arg));
                    index++;
                }
                value = values.ToArray();
                break;
            }
            case NArgs.VALUE_ONE: {
                index++;
                value = Parse(arguments[index++]);
                break;
            }
            default: {
                int len = nargs.Value;
                if (index + len >= arguments.Length) { throw new ArgumentException("Not enough values"); }
                index++;
                value = Enumerable.Range(index, len).Select(i => Parse(arguments[i])).ToArray();
                index += len;
                break;
            }
        }
        return true;
    }

    internal static readonly Boolean DefaultHelp = new("help", true) {
        ArgNumber = 0,
        Flags = new string[] { "-h" },
        Help = "Prints this help message."
    };
}

public class Argument<T> : Argument {
    public new Nullable<T> Constant {
        get => _constant.Cast<T>();
        set => _constant = value;
    }
    public new Nullable<T> Default {
        get => _default.Cast<T>();
        set => _default = value;
    }
    public Argument(string name, T? constant) : base(name, constant) { Default = default(T); }
    public Argument(string name, Parser? type) : base(name, type) { Default = default(T); }
    public new T? Parse(string value) => ResolveChecked(Ignore(value));
    protected override object? Resolve(string value) => ResolveChecked(value);
    protected virtual T? ResolveChecked(string value) =>
        Constant.HasValue ? Constant.Value :
        (T?)(_type ?? throw new NullReferenceException(nameof(Type))).DynamicInvoke(value);
    public void SetConstant(Nullable<T> constant) {
        if (!constant.HasValue) { CheckDelegate(_type?.Method); }
        _constant = constant;
    }

    protected override void CheckDelegate(MethodInfo? method) {
        if (Constant.HasValue) { return; }
        base.CheckDelegate(method);
        if (!CheckReturn(method, typeof(T))) { throw new ArgumentException($"Type must return a value of type {typeof(T)} but returned {method?.ReturnType}"); }
    }

    public new delegate T? Parser(string value);
}
