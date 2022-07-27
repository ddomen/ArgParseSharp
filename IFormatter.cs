using System.Text;

namespace ArgParseSharp;

public interface IHelpFormatter {
    public void Groups(IGroup group);
    public void Positionals(IEnumerable<Argument> positionals);
    public void Optionals(IEnumerable<Argument> optionals, bool help);
    public void Flush(StringBuilder builder);
    public void Reset();
}


public interface IUsageFormatter {
    public void Program(string program);
    public void Positionals(IEnumerable<Argument> positionals);
    public void Optionals(IEnumerable<Argument> optionals, bool help);
    public void Flush(StringBuilder builder);
    public void Reset();
}

public interface IFormatter : IHelpFormatter, IUsageFormatter {

}
