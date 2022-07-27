namespace ArgParseSharp;

public readonly struct ParsedElement {
    public readonly string? Name;
    public readonly string? Argument;
    public readonly object? Value;
    public bool IsEmpty => Name is null && Argument is null && Value is null;

    public ParsedElement(string? name, object? value, IEnumerable<string>? argument) :
        this(name, value, string.Join(" ", argument ?? Enumerable.Empty<string>())) { }
    public ParsedElement(string? name, object? value) : this(name, value, (string?)null) { }
    public ParsedElement(string? name, object? value, string? argument) {
        Name = name;
        Argument = argument;
        Value = value;
    }

    public override bool Equals(object obj) => Equals(Value, obj);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string ToString() =>
        $"{(Name is not null ? $"{Name}=" : "")}{Value} ({Argument})";
}
