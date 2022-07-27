namespace ArgParseSharp;

public readonly struct Nullable<T> {
    public readonly T? Value;
    public readonly bool HasValue;

    public Nullable(T? value) {
        Value = value;
        HasValue = true;
    }

    internal R? As<R>() =>
        HasValue ?
            Value is R r ? r :
            typeof(IConvertible).IsAssignableFrom(typeof(R)) && Value is IConvertible c ? (R)c.ToType(typeof(R), null) :
        default : default
        ;

    public override bool Equals(object obj) =>
        obj is T t ? HasValue && Equals(Value, t) :
        obj is Nullable<T> n && Equals(n);

    public override int GetHashCode() => HasValue ? Value?.GetHashCode() ?? 0 : 0;
    public override string ToString() => $"{Value}";

    public bool Equals(Nullable<T> nullable) =>
        HasValue ? nullable.HasValue && Equals(Value, nullable.Value) : !nullable.HasValue;

    public static implicit operator T?(Nullable<T> n) => n.Value;
    public static implicit operator Nullable<T>(T? value) => new(value);

    private static bool Equals(T? l, T? r) =>
        (l is null && r is null) || (l is not null && l.Equals(r));


    public static readonly Nullable<T> Empty = new();
}
