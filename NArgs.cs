namespace ArgParseSharp;

public readonly struct NArgs {
    public readonly int Value;
    public NArgs(int value) =>
        Value = value < VALUE_ONE_OR_MORE ? throw new ArgumentOutOfRangeException(nameof(value)) : value;

    public override bool Equals(object obj) =>
        obj is int i ? Value == i :
        obj is NArgs other && Equals(other);

    public bool Equals(NArgs n) => Value == n.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static implicit operator int(NArgs n) => n.Value;
    public static implicit operator NArgs(int c) => new(c);

    public static bool operator ==(NArgs l, NArgs r) => l.Equals(r);
    public static bool operator !=(NArgs l, NArgs r) => !l.Equals(r);

    public const int VALUE_ONE = 1;
    public const int VALUE_ZERO = 0;
    public const int VALUE_OPTIONAL = -1;
    public const int VALUE_ZERO_OR_MORE = -2;
    public const int VALUE_ONE_OR_MORE = -3;

    public static readonly NArgs ONE = new(VALUE_ONE);
    public static readonly NArgs ZERO = new(VALUE_ZERO);
    public static readonly NArgs OPTIONAL = new(VALUE_OPTIONAL);
    public static readonly NArgs ZERO_OR_MORE = new(VALUE_ZERO_OR_MORE);
    public static readonly NArgs ONE_OR_MORE = new(VALUE_ONE_OR_MORE);
}
