namespace ArgParseSharp;

public interface IGroup : IReadOnlyDictionary<string, IGroup> {
    public IEnumerable<Argument> Positionals { get; }
    public IEnumerable<Argument> Optionals { get; }
}