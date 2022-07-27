using System.Text;

namespace ArgParseSharp;
internal class FormatterDefault : IFormatter {

    protected StringBuilder sb = new();

    private void Groups(IGroup group, int depth = 0) {
        string indent = new('\t', depth);
        foreach (KeyValuePair<string, IGroup> k in group) {
            sb.AppendLine($"{indent}{k.Key}:");
            Groups(k.Value, depth + 1);
        }
        if (depth > 0) {
            HelpPositionals(group.Positionals, depth);
            HelpOptionals(group.Optionals, false, depth);
        }
    }

    private void HelpOptionals(IEnumerable<Argument> optionals, bool help, int depth = 0) {
        string indent = new('\t', depth + 1);
        if (depth is 0) { sb.AppendLine("optional arguments:"); }
        if (help) { sb.AppendLine($"{indent}-h, --help\t\tshow this help message and exit"); }
        foreach (Argument arg in optionals) {
            sb.AppendLine($"{indent}{string.Join(", ", MetaNames(arg))}\t\t{arg.Help}");
        }
    }

    private void HelpPositionals(IEnumerable<Argument> positionals, int depth = 0) {
        string indent = new('\t', depth + 1);
        if (depth is 0) { sb.AppendLine("\npositional arguments:"); }
        foreach (Argument arg in positionals) { sb.AppendLine($"{indent}{arg.Name}\t\t{arg.Help}"); }
    }

    private IEnumerable<string> MetaNames(Argument arg) {
        string meta = string.Empty;
        NArgs nargs = arg.ArgNumber ?? new(1);
        if (arg.MetaVariables is not null) { meta = string.Join(" ", arg.MetaVariables.Where(v => !string.IsNullOrWhiteSpace(v))); }
        else if (nargs.Value is 1) { meta = $" {arg.Name.ToUpper()}"; }
        else if (nargs.Value is NArgs.VALUE_OPTIONAL) { meta = $" {arg.Name.ToUpper()}?"; }
        else if (nargs.Value is NArgs.VALUE_ZERO_OR_MORE) { meta = $" {arg.Name.ToUpper()}*"; }
        else if (nargs.Value is NArgs.VALUE_ONE_OR_MORE) { meta = $" {arg.Name.ToUpper()}+"; }
        return (arg.Flags ?? Array.Empty<string>()).Prepend(arg.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => $"{n}{meta}");
    }

    void IHelpFormatter.Groups(IGroup group) => Groups(group, 0);
    void IHelpFormatter.Positionals(IEnumerable<Argument> positionals) => HelpPositionals(positionals, 0);
    void IHelpFormatter.Optionals(IEnumerable<Argument> optionals, bool help) => HelpOptionals(optionals, help, 0);
    void IHelpFormatter.Reset() {
        sb.Clear();
        sb.Append("\n\n");
    }

    void IUsageFormatter.Positionals(IEnumerable<Argument> positionals) {
        foreach (Argument arg in positionals) { sb.Append(" " + arg.Name); }
    }
    void IUsageFormatter.Optionals(IEnumerable<Argument> optionals, bool help) {
        if (help) { sb.Append(" [-h]"); }
        foreach (Argument arg in optionals) { sb.Append($" [{MetaNames(arg).First()}]"); }
    }
    void IUsageFormatter.Program(string program) => sb.Append($" {program} ");
    void IUsageFormatter.Reset() {
        sb.Clear();
        sb.Append("usage: ");
    }

    public void Flush(StringBuilder builder) {
        builder.Append(sb);
        sb.Clear();
    }

}