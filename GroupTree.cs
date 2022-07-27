namespace ArgParseSharp;

internal sealed class GroupTree : Dictionary<string, GroupTree>, IGroup {
    public readonly List<Argument> Content = new();
    public string PrefixChars;
    IGroup IReadOnlyDictionary<string, IGroup>.this[string key] => this[key];
    public IEnumerable<Argument> Positionals => Content.Where(c => c.IsPositional(PrefixChars));
    public IEnumerable<Argument> Optionals => Content.Where(c => !c.IsPositional(PrefixChars));
    public GroupTree(string prefixs) => PrefixChars = prefixs;
    IEnumerable<string> IReadOnlyDictionary<string, IGroup>.Keys => Keys;
    IEnumerable<IGroup> IReadOnlyDictionary<string, IGroup>.Values => Values;
    public bool TryGetValue(string key, out IGroup value) {
        value = null!;
        if (!TryGetValue(key, out GroupTree g)) { return false; }
        value = g;
        return true;
    }
    IEnumerator<KeyValuePair<string, IGroup>> IEnumerable<KeyValuePair<string, IGroup>>.GetEnumerator() {
        foreach (KeyValuePair<string, GroupTree> k in this) { yield return new KeyValuePair<string, IGroup>(k.Key, k.Value); }
    }

    public GroupTree Spawn(string name) {
        if (!TryGetValue(name, out GroupTree res)) {
            Add(name, res = new GroupTree(PrefixChars));
        }
        return res;
    }
}
