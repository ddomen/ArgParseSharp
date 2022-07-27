using System.Collections;

namespace ArgParseSharp;

public sealed class ParsedArguments : IReadOnlyDictionary<string, ParsedElement> {
    private readonly Dictionary<string, ParsedElement> _args;
    private readonly ParsedElement[] _extras;
    internal ParsedArguments(Dictionary<string, ParsedElement> args, ParsedElement[] extras) {
        _args = args;
        _extras = extras;
    }

    public ParsedElement[] Extras => (ParsedElement[])_extras.Clone();

    public int Count => _args.Count;
    public ParsedElement this[string key] => _args.TryGetValue(key, out ParsedElement value) ? value : default;
    public IEnumerable<string> Keys => _args.Keys;
    public IEnumerable<ParsedElement> Values => _args.Values;
    public bool ContainsKey(string key) => _args.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, ParsedElement>> GetEnumerator() => _args.GetEnumerator();
    public bool TryGetValue(string key, out ParsedElement value) => _args.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
