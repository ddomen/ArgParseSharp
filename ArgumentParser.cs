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
    public ArgumentParser[]? Parents { get; set; }
    public ArgumentParser[]? Children { get; set; }
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

    public void AddSubParser(ArgumentParser parser) {
        if (parser is null) { throw new ArgumentNullException(nameof(parser)); }
        Children = (Children ?? Array.Empty<ArgumentParser>()).Append(parser).ToArray();
    }

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

    public T ParseArgs<T>(Stream target) => ParseArgs<T>(ReadArgs(target));
    public T ParseArgs<T>(params string?[]? args) => ParseArgs<T>(out _, args);
    public T ParseArgs<T>(Stream target, out ParsedElement[] extras) => ParseArgs<T>(out extras, ReadArgs(target));
    public T ParseArgs<T>(out ParsedElement[] extras, params string?[]? args) {
        ParsedArguments parsed = ParseArgs(args);
        T result = Activator.CreateInstance<T>();
        List<ParsedElement> extrasList = parsed.Extras.ToList();
        MemberInfo[] members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
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

    public const string DEFAULT_PREFIX_CHARS = "-";
    protected readonly Regex ArgLineSplitter = new(@"\s+", RegexOptions.Compiled);
    protected readonly Regex MemberNameChecker = new(@"[a-zA-Z][a-zA-Z0-9_]*", RegexOptions.Compiled);
}