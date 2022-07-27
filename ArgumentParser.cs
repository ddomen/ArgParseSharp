using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ArgParseSharp;

public class ArgumentParser {
    public string? Usage { get; set; }
    public string? Epilog { get; set; }
    public string? Program { get; set; }
    public string? Description { get; set; }
    public string? PrefixChars { get; set; } = DEFAULT_PREFIX_CHARS;
    public string? FromFilePrefixChars { get; set; }
    public string? ArgumentDefault { get; set; }
    public IHelpFormatter? HelpFormatter { get; set; }
    public IUsageFormatter? UsageFormatter { get; set; }
    public IFormatter? Formatter {
        get => HelpFormatter == UsageFormatter && HelpFormatter is IFormatter f ? f : null;
        set { HelpFormatter = value; UsageFormatter = value; }
    }
    //public ArgumentParser[]? Parents { get; set; }
    //public ArgumentParser[]? Children { get; set; }
    public bool AddHelp { get; set; } = true;
    public bool AllowExtras { get; set; } = true;
    public bool ThrowOnConflicts { get; set; } = true;
    public bool IgnoreCase { get; set; } = true;
    public bool AllowAbbreviations { get; set; }
    public bool CheckedMemberResolution { get; set; }
    private readonly List<Argument> _args = new();

    private bool HandleConflict(Argument argument, Argument check, string flag) =>
        ThrowOnConflicts ? throw new ArgumentException($"Argument conflicting option: {flag}") :
        true;

    private static bool IsNullableType(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    private static ConstructorInfo? RetrieveCtor(Type type, Type?[] args) {
        List<ConstructorInfo> ctors = new();
        bool mustInfer = args.Contains(null);
        ParameterInfo[] ps;
        ParameterInfo p;
        Type? t;
        Type pt;
        bool exact;
        bool assignable;

        int argLen = args.Length;
        foreach (ConstructorInfo ctor in type.GetConstructors()) {
            ps = ctor.GetParameters();
            assignable = true;
            exact = ps.Length == argLen;
            for (int i = 0; i < ps.Length; ++i) {
                p = ps[i];
                pt = p.ParameterType;
                if (i < argLen) { t = args[i]; }
                else if (!p.IsOptional) { assignable = false; break; }
                else { t = pt; exact = false; }
                if (t is null) {
                    if (!IsNullableType(pt)) { assignable = false; break; }
                    t = pt;
                }
                if (!pt.IsAssignableFrom(t)) { assignable = false; break; }
                exact = exact && pt == t;
            }
            if (exact) { return ctor; }
            if (assignable) { ctors.Add(ctor); }
        }
        return ctors.Count == 1 ? ctors[0] : null;
    }
    private static Argument CreateArgument(Type argumentType, params object?[]? values) {
        if (argumentType is null) { throw new ArgumentNullException(nameof(argumentType)); }
        if (!typeof(Argument).IsAssignableFrom(argumentType)) { throw new ArgumentException($"The given type {argumentType} does not extends {typeof(Argument)}"); }
        ConstructorInfo? ctor = RetrieveCtor(
            argumentType,
            (values ?? Array.Empty<object?>()).Select(v => v?.GetType()).ToArray()
        );
        return ctor is null ? throw new ArgumentException("No constructor found for the given inptus") : (Argument)ctor.Invoke(values);
    }
    private static A CreateArgument<A>(params object?[]? values) where A : Argument => (A)CreateArgument(typeof(A), values);

    public void AddArgument(Argument argument) {
        if (argument is null) { throw new ArgumentNullException(nameof(argument)); }
        string[] ids = argument.Identifiers.ToArray();
        foreach (Argument arg in _args.ToArray()) {
            foreach (string argFlag in arg.Identifiers) {
                if (ids.Contains(argFlag) && HandleConflict(arg, argument, argFlag)) { _args.Remove(arg); }
            }
        }
        _args.Add(argument);
    }
    public void AddArgument(Type argumentType, params object?[]? values) =>
        AddArgument(CreateArgument(argumentType, values));
    public void AddArgument(Type argumentType, Action<Argument> configurator, params object?[]? values) {
        Argument arg = CreateArgument(argumentType, values);
        configurator?.Invoke(arg);
        AddArgument(arg);
    }
    public void AddArgument<T>(string name, T? value) => AddArgument(new Argument<T>(name, value));
    public void AddArgument<A>(string name, Action<A>? configurator = null) where A : Argument {
        A? arg = CreateArgument<A>(name);
        configurator?.Invoke(arg);
        AddArgument(arg);
    }
    public void AddArgument<A>(Action<A>? configurator = null) where A : Argument {
        A? arg = CreateArgument<A>();
        configurator?.Invoke(arg);
        AddArgument(arg);
    }
    public void AddArgument<A>(params object?[] constructorArgs) where A : Argument {
        A? arg = CreateArgument<A>(constructorArgs);
        AddArgument(arg);
    }

    public void Boolean(string name, bool value = true) => AddArgument(new Argument.Boolean(name, value));
    public void Boolean(string name, Argument.Boolean.Parser? parser) => AddArgument(new Argument.Boolean(name, parser));
    public void Byte(string name, byte value) => AddArgument(new Argument.Byte(name, value));
    public void Byte(string name, Argument.Byte.Parser? parser = null) => AddArgument(new Argument.Byte(name, parser));
    public void SByte(string name, sbyte value) => AddArgument(new Argument.SByte(name, value));
    public void SByte(string name, Argument.SByte.Parser? parser = null) => AddArgument(new Argument.SByte(name, parser));
    public void Char(string name, char value) => AddArgument(new Argument.Char(name, value));
    public void Char(string name, Argument.Char.Parser? parser = null) => AddArgument(new Argument.Char(name, parser));
    public void UShort(string name, ushort value) => AddArgument(new Argument.UShort(name, value));
    public void UShort(string name, Argument.UShort.Parser? parser = null) => AddArgument(new Argument.UShort(name, parser));
    public void Short(string name, short value) => AddArgument(new Argument.Short(name, value));
    public void Short(string name, Argument.Short.Parser? parser = null) => AddArgument(new Argument.Short(name, parser));
    public void UInt(string name, uint value) => AddArgument(new Argument.UInt(name, value));
    public void UInt(string name, Argument.UInt.Parser? parser = null) => AddArgument(new Argument.UInt(name, parser));
    public void Int(string name, int value) => AddArgument(new Argument.Int(name, value));
    public void Int(string name, Argument.Int.Parser? parser = null) => AddArgument(new Argument.Int(name, parser));
    public void ULong(string name, ulong value) => AddArgument(new Argument.ULong(name, value));
    public void ULong(string name, Argument.ULong.Parser? parser = null) => AddArgument(new Argument.ULong(name, parser));
    public void Long(string name, long value) => AddArgument(new Argument.Long(name, value));
    public void Long(string name, Argument.Long.Parser? parser = null) => AddArgument(new Argument.Long(name, parser));
    public void Float(string name, float value) => AddArgument(new Argument.Float(name, value));
    public void Float(string name, Argument.Float.Parser? parser = null) => AddArgument(new Argument.Float(name, parser));
    public void Double(string name, double value) => AddArgument(new Argument.Double(name, value));
    public void Double(string name, Argument.Double.Parser? parser = null) => AddArgument(new Argument.Double(name, parser));
    public void Version(string? name = null) => AddArgument(new Argument.Version(name));
    public void File(string name, FileInfo? value) => AddArgument(new Argument.File(name, value));
    public void File(string name, Argument.File.Parser? parser = null) => AddArgument(new Argument.File(name, parser));

    //public void AddSubParser(ArgumentParser parser) {
    //    if (parser is null) { throw new ArgumentNullException(nameof(parser)); }
    //    Children = (Children ?? Array.Empty<ArgumentParser>()).Append(parser).ToArray();
    //}

    public void AddArgumentsFromType<T>() => AddArgumentsFromType(typeof(T));
    public void AddArgumentsFromType(Type target) {
        if (target is null) { throw new ArgumentNullException(nameof(target)); }
        foreach (MemberInfo m in target.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            ArgumentAttribtue? attr = m.GetCustomAttribute<ArgumentAttribtue>();
            if (attr is null) { continue; }
            AddArgument(attr.CreateArgument(m));
        }
    }

    public ParsedArguments ParseKnwon(Stream target) => ParseKnown(ReadArgs(target));
    public ParsedArguments ParseKnown(params string?[]? args) {
        string[] rArgs = (args ?? Environment.GetCommandLineArgs()).Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
        Dictionary<string, ParsedElement> parsed = new();
        List<ParsedElement> extras = new();
        string prefixes = PrefixChars ?? DEFAULT_PREFIX_CHARS;
        int len = rArgs.Length;
        bool isExtra = true;

        HashSet<Argument> parsedArgs = new();
        Argument[] mArgs = (AddHelp ? _args.Prepend(Argument.DefaultHelp) : _args).ToArray();
        int iref = 0;
        for (int i = 0; i < len;) {
            isExtra = true;
            iref = i;
            foreach (Argument arg in mArgs) {
                if (arg.TryResolve(rArgs, ref i, out object? p, prefixes, IgnoreCase)) {
                    parsed.Add(
                        MemberNameChecker.Match(arg.MemberName ?? arg.Name).Value,
                        new(arg.Name, p, rArgs.Skip(iref).Take(i - iref))
                    );
                    isExtra = false;
                    parsedArgs.Add(arg);
                    break;
                }
            }
            if (isExtra) { extras.Add(new(rArgs[i++], null)); }
        }

        foreach (Argument arg in mArgs) {
            if (arg.Default.HasValue && !parsedArgs.Contains(arg)) {
                parsed.Add(
                    MemberNameChecker.Match(arg.MemberName ?? arg.Name).Value,
                    new(arg.Name, arg.Default.Value)
                );
            }
        }

        return new(parsed, extras.ToArray());

    }

    public ParsedArguments ParseArgs(Stream target) => ParseArgs(ReadArgs(target));
    public ParsedArguments ParseArgs(params string?[]? args) {
        ParsedArguments result = ParseKnown(args);
        if (AddHelp && result.TryGetValue("help", out ParsedElement v) && v.Value is bool b && b) {
            PrintHelp(null);
            Exit();
        }
        return result.Extras.Length > 0 && !AllowExtras ?
                throw new ArgumentException($"Invalid arguments: {string.Join(", ", result.Extras)}") :
                result;
    }


    public object ParseArgs(Type returnType, Stream target) => ParseArgs(returnType, ReadArgs(target));
    public object ParseArgs(Type returnType, params string?[]? args) => ParseArgs(returnType, out _, args);
    public object ParseArgs(Type returnType, Stream target, out ParsedElement[] extras) => ParseArgs(returnType, out extras, ReadArgs(target));
    public object ParseArgs(Type returnType, out ParsedElement[] extras, params string?[]? args) {
        ParsedArguments parsed = ParseArgs(args);
        object result = Activator.CreateInstance(returnType);
        List<ParsedElement> extrasList = parsed.Extras.ToList();
        MemberInfo[] members = returnType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property)
            .ToArray();
        foreach (KeyValuePair<string, ParsedElement> k in parsed) {
            MemberInfo? target = members.FirstOrDefault(m => m.Name.Equals(k.Key, StringComparison.OrdinalIgnoreCase));
            if (target is FieldInfo field) {
                try { field.SetValue(result, k.Value.Value); }
                catch { if (CheckedMemberResolution) { throw; } }
            }
            else if (target is PropertyInfo property) {
                try { property.SetValue(result, k.Value.Value); }
                catch { if (CheckedMemberResolution) { throw; } }
            }
            else if (CheckedMemberResolution) { throw new ArgumentException($"Invalid argument resolution: {k.Key}"); }
            else { extrasList.Add(new(null, k.Value, k.Key)); }
        }
        extras = extrasList.ToArray();
        return result;
    }

    public T ParseArgs<T>(Stream target) => (T)ParseArgs(typeof(T), target);
    public T ParseArgs<T>(params string?[]? args) => (T)ParseArgs(typeof(T), args);
    public T ParseArgs<T>(Stream target, out ParsedElement[] extras) => (T)ParseArgs(typeof(T), target, out extras);
    public T ParseArgs<T>(out ParsedElement[] extras, params string?[]? args) => (T)ParseArgs(typeof(T), out extras, args);

    public virtual void Exit(int status = 0, string? message = null, Stream? target = null) {
        if (message is not null) { PrintError(message, target); }
        Environment.Exit(status);
    }
    public void Exit(string message, Stream? target = null) => Exit(0, message, target);

    public void PrintError(string message, Stream? target = null) {
        if (message is not null) { WriteToStream(target, message + "\n"); }
    }

    public string PrintUsage() {
        StringBuilder sb = new();
        string prefixs = PrefixChars ?? DEFAULT_PREFIX_CHARS;
        List<Argument> positionals = new();
        List<Argument> optionals = new();
        foreach (Argument arg in _args) {
            if (arg.IsPositional(prefixs)) { positionals.Add(arg); }
            else { optionals.Add(arg); }
        }
        IUsageFormatter formatter = UsageFormatter ?? new FormatterDefault();
        formatter.Reset();
        formatter.Program(Program ?? Assembly.GetEntryAssembly().GetName().Name);
        formatter.Optionals(optionals, AddHelp);
        formatter.Positionals(positionals);
        formatter.Flush(sb);
        return sb.ToString();
    }


    public string PrintHelp() {
        StringBuilder sb = new(PrintUsage());
        string prefixs = PrefixChars ?? DEFAULT_PREFIX_CHARS;
        List<Argument> optionals = new();
        List<Argument> positionals = new();
        GroupTree groups = new(prefixs);
        GroupTree current;
        foreach (Argument arg in _args) {
            if (!arg.IsPositional(prefixs)) {
                optionals.Add(arg);
                continue;
            }
            string[] hs = arg.GroupHierarchy;
            if (hs.Length > 0) {
                current = groups;
                foreach (string h in arg.GroupHierarchy) {
                    current = current.Spawn(h);
                }
                current.Content.Add(arg);
                continue;
            }
            positionals.Add(arg);
        }
        IHelpFormatter formatter = HelpFormatter ?? new FormatterDefault();
        formatter.Reset();
        formatter.Optionals(optionals, AddHelp);
        formatter.Positionals(positionals);
        formatter.Groups(groups);
        formatter.Flush(sb);
        return sb.ToString();
    }

    public void PrintUsage(Stream? target) =>
        WriteToStream(target, PrintUsage());

    public void PrintHelp(Stream? target) =>
        WriteToStream(target, PrintHelp());

    private void WriteToStream(Stream? target, string text) {
        if (target is null) { Console.Write(text); return; }
        using StreamWriter sw = new(target);
        sw.Write(text);
        sw.Flush();
    }

    protected virtual string ReadStream(Stream target) {
        using StreamReader sr = new(target);
        return sr.ReadToEnd();
    }

    protected virtual string[] ConvertArgLineToArgs(string argLine) =>
        ArgLineSplitter.Split(argLine);

    private string[] ReadArgs(Stream target) =>
        ConvertArgLineToArgs(ReadStream(target ?? throw new ArgumentNullException(nameof(target))));

    public static ArgumentParser FromType<T>() => FromType(typeof(T));
    public static ArgumentParser FromType(Type target) {
        ArgumentParser p = new();
        p.AddArgumentsFromType(target);
        return p;
    }

    public static T AutoParse<T>(params string?[]? args) => FromType(typeof(T)).ParseArgs<T>(args);
    public static T AutoParse<T>(out ParsedElement[] extras, params string?[]? args) => FromType(typeof(T)).ParseArgs<T>(out extras, args);
    public static T AutoParse<T>(Stream target) => FromType(typeof(T)).ParseArgs<T>(target);
    public static T AutoParse<T>(Stream target, out ParsedElement[] extras) => FromType(typeof(T)).ParseArgs<T>(target, out extras);

    public static object AutoParse(Type type, params string?[]? args) => FromType(type).ParseArgs(type, args);
    public static object AutoParse(Type type, out ParsedElement[] extras, params string?[]? args) => FromType(type).ParseArgs(type, out extras, args);
    public static object AutoParse(Type type, Stream target) => FromType(type).ParseArgs(type, target);
    public static object AutoParse(Type type, Stream target, out ParsedElement[] extras) => FromType(type).ParseArgs(type, target, out extras);


    public const string DEFAULT_PREFIX_CHARS = "-";
    protected readonly Regex ArgLineSplitter = new(@"\s+", RegexOptions.Compiled);
    protected readonly Regex MemberNameChecker = new(@"[a-zA-Z][a-zA-Z0-9_]*", RegexOptions.Compiled);
}